namespace Common.Infrastructure.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Attributes;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using RestSharp;
    using StackExchange.Redis;

    public abstract class PublicApiController : ApiController
    {
        protected const int DefaultStatusCaching = 0;
    }

    [ApiController]
    [RejectInvalidQueryParametersFilter]
    [SupportUrlFormat]
    public abstract class ApiController<T> : PublicApiController
    {
        private const string ValueKey = "value";
        private const string HeadersKey = "headers";
        private const string LastModifiedKey = "lastModified";
        private const string SetByRegistryKey = "setByRegistry";

        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<T> _logger;

        protected ApiController(
            ConnectionMultiplexerProvider redis,
            ILogger<T> logger)
        {
            _redis = redis.GetConnectionMultiplexer();
            _logger = logger;
        }

        protected async Task<BackendResponse> GetFromCacheThenFromBackendAsync(
            AcceptType acceptType,
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            string cacheKey,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            if (_redis != null)
            {
                var key = $"{cacheKey}.{acceptType}".ToLowerInvariant();

                try
                {
                    var db = _redis.GetDatabase();

                    var cachedValues =
                        await db.HashGetAllAsync(
                            key,
                            CommandFlags.PreferReplica);

                    if (cachedValues.Length > 0)
                    {
                        var cachedSetByRegistry = cachedValues.First(x => x.Name.Equals(SetByRegistryKey));

                        if (cachedSetByRegistry.Value == true.ToString(CultureInfo.InvariantCulture))
                        {
                            var cachedValue = cachedValues.FirstOrDefault(x => x.Name.Equals(ValueKey));
                            var cachedHeaders = cachedValues.FirstOrDefault(x => x.Name.Equals(HeadersKey));
                            var cachedLastModified = cachedValues.FirstOrDefault(x => x.Name.Equals(LastModifiedKey));

                            var headers = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(cachedHeaders.Value);
                            headers.TryGetValue(AddVersionHeaderMiddleware.HeaderName, out var downstreamVersion);

                            return new BackendResponse(
                                cachedValue.Value,
                                downstreamVersion?.First(),
                                DateTimeOffset.ParseExact(
                                    cachedLastModified.Value,
                                    "O",
                                    CultureInfo.InvariantCulture),
                                acceptType.ToMimeTypeString(),
                                true,
                                Enumerable.Empty<KeyValuePair<string, StringValues>>());
                        }

                        _logger.LogError("Failed to retrieve record {Record} from Redis, cached values not set by registry.", key);
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve record {Record} from Redis, no cached values.", key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve record {Record} from Redis.", key);
                }
            }

            return await GetFromBackendAsync(
                restClient,
                createBackendRequestFunc,
                acceptType,
                handleNotOkResponseAction,
                cancellationToken);
        }

        protected static async Task<BackendResponse> GetFromBackendWithBadRequestAsync(
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var contentType = acceptType.ToMimeTypeString();

            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, contentType);

            var response = await ExecuteRequestAsync(restClient, backendRequest, cancellationToken);

            var downstreamVersion = response
                .Headers
                .FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase));

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BackendResponse(
                    GetPublicContentValue(response, problemDetailsHelper),
                    downstreamVersion?.Value.ToString(),
                    DateTimeOffset.UtcNow,
                    response.ContentType,
                    false,
                    response.HeadersToKeyValuePairs(),
                    response.StatusCode);
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new BackendResponse(
                    response.Content,
                    downstreamVersion?.Value.ToString(),
                    DateTimeOffset.UtcNow,
                    contentType,
                    false,
                    response.HeadersToKeyValuePairs(),
                    response.StatusCode);
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiException("Fout bij de bron.", (int)response.StatusCode, response.ErrorException);
        }

        protected static async Task<IBackendResponse> GetFromBackendWithBadRequestAsync(
            HttpClient httpClient,
            Func<HttpRequestMessage> createBackendRequestFunc,
            Action<HttpStatusCode> handleNotOkResponseAction,
            ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var backendRequest = createBackendRequestFunc();
            var response = await httpClient.SendAsync(backendRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
                var contentType = response.Content.Headers.ContentType?.ToString();
                var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

                return new StreamingBackendResponse(
                    contentType,
                    contentDisposition,
                    responseStream,
                    response.HeadersToKeyValuePairs());
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContentType = response.Content.Headers.ContentType?.ToString();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                return new BackendResponse(
                    GetPublicContentValue(response, responseContent, problemDetailsHelper),
                    null,
                    DateTimeOffset.UtcNow,
                    responseContentType,
                    false,
                    response.HeadersToKeyValuePairs(),
                    response.StatusCode);
            }

            handleNotOkResponseAction(response.StatusCode);
            throw new ApiException("Fout bij de bron.", (int)response.StatusCode);
        }

        private static string GetPublicContentValue(IRestResponse response, ProblemDetailsHelper helper)
        {
            var problemDetails = response.GetProblemDetails();
            if (string.IsNullOrWhiteSpace(problemDetails.ProblemTypeUri))
                return response.Content;

            string Encode(string value)
                => (response.ContentType.Contains("xml", StringComparison.InvariantCultureIgnoreCase)
                       ? new XElement(XName.Get("dummy"), value).LastNode?.ToString()
                       : value)
                   ?? string.Empty;

            return response
                .Content
                .Replace(
                    Encode(problemDetails.ProblemTypeUri),
                    Encode(helper.RewriteExceptionTypeFrom(problemDetails)));
        }

        private static string GetPublicContentValue(HttpResponseMessage response, string responseContent, ProblemDetailsHelper helper)
        {
            var problemDetails = response.GetProblemDetails(responseContent);
            if (string.IsNullOrWhiteSpace(problemDetails.ProblemTypeUri))
                return responseContent;

            string Encode(string value)
                => (response.Content.Headers.ContentType.MediaType.Contains("xml", StringComparison.InvariantCultureIgnoreCase)
                       ? new XElement(XName.Get("dummy"), value).LastNode?.ToString()
                       : value)
                   ?? string.Empty;

            return responseContent
                .Replace(
                    Encode(problemDetails.ProblemTypeUri),
                    Encode(helper.RewriteExceptionTypeFrom(problemDetails)));
        }

        protected static async Task<BackendResponse> GetFromBackendAsync(
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            var contentType = acceptType.ToMimeTypeString();

            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, contentType);

            var response = await ExecuteRequestAsync(restClient, backendRequest, cancellationToken);

            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                var downstreamVersion = response
                    .Headers
                    .FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase));

                return new BackendResponse(
                    response.Content,
                    downstreamVersion?.Value.ToString(),
                    DateTimeOffset.UtcNow,
                    contentType,
                    false,
                    response.HeadersToKeyValuePairs());
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiProblemDetailsException("Fout bij de bron.", (int)response.StatusCode, response.GetProblemDetails(), response.ErrorException);
        }

        private static async Task<IRestResponse> ExecuteRequestAsync(
            IRestClient restClient,
            IRestRequest backendRequest,
            CancellationToken cancellationToken)
        {
            var response = await restClient.ExecuteAsync(backendRequest, cancellationToken);

            // Api gateway hard limit: https://docs.aws.amazon.com/apigateway/latest/developerguide/limits.html
            if (response.Content.Length > 10_000_000)
                throw new ApiException(
                    "Response is te groot, probeer de 'limit' parameter te verkleinen en probeer opnieuw.",
                    StatusCodes.Status500InternalServerError);

            return response;
        }
    }
}
