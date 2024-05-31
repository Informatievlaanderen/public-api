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
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using RestSharp;
    using StackExchange.Redis;

    public abstract class PublicApiController : ApiController
    {
        protected const int DefaultStatusCaching = 0;
    }

    [ApiController]
    [RejectInvalidQueryParametersFilter]
    [SupportUrlFormat] //to ensure errors are returned in the format which the controller [Produces]
    public abstract class ApiController<T> : PublicApiController
    {
        private const string ETagKey = "eTag";
        private const string ValueKey = "value";
        private const string HeadersKey = "headers";
        private const string LastModifiedKey = "lastModified";
        private const string SetByRegistryKey = "setByRegistry";

        private readonly IConnectionMultiplexer? _redis;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<T> _logger;

        protected ApiController(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<T> logger)
        {
            _redis = redis.GetConnectionMultiplexer();
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected async Task<BackendResponse> GetFromCacheThenFromBackendAsync(
            AcceptType acceptType,
            RestClient restClient,
            Func<RestRequest> createBackendRequestFunc,
            string cacheKey,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            if (_redis != null)
            {
                var key = $"{cacheKey}.{acceptType}".ToLowerInvariant();
                var cachedResponse = await GetFromCacheAsync(key, acceptType);

                if (cachedResponse is not null)
                {
                    return cachedResponse;
                }

                if(acceptType is AcceptType.Json or AcceptType.Ld)
                {
                    _logger.LogInformation("Failed to retrieve record {Record} from Redis, trying to retrieve JSON-LD instead.", key);

                    key = $"{cacheKey}.jsonld".ToLowerInvariant();
                    cachedResponse = await GetFromCacheAsync(key, AcceptType.JsonLd);

                    if (cachedResponse is not null)
                    {
                        return cachedResponse;
                    }
                }

                if (cachedResponse is null)
                {
                    _logger.LogWarning("Failed to retrieve record {Record} from Redis, no cached values.", key);
                }
            }

            return await GetFromBackendAsync(
                restClient,
                createBackendRequestFunc,
                acceptType,
                handleNotOkResponseAction,
                cancellationToken);
        }

        private async Task<BackendResponse?> GetFromCacheAsync(string key, AcceptType acceptType)
        {
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
                        var cachedETag = cachedValues.FirstOrDefault(x => x.Name.Equals(ETagKey));

                        var headers = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(cachedHeaders.Value) ?? new Dictionary<string, string[]>();
                        headers.TryGetValue(AddVersionHeaderMiddleware.HeaderName, out var downstreamVersion);
                        if (cachedETag.Value.HasValue)
                        {
                            headers.Add(HeaderNames.ETag, new[] { cachedETag.Value.ToString() });
                        }

                        return new BackendResponse(
                            cachedValue.Value,
                            downstreamVersion?.First(),
                            DateTimeOffset.ParseExact(
                                cachedLastModified.Value,
                                "O",
                                CultureInfo.InvariantCulture),
                            acceptType.ToMimeTypeString(),
                            true,
                            headers.Select(x => new KeyValuePair<string, StringValues>(x.Key, new StringValues(x.Value))));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve record {Record} from Redis.", key);
            }

            return null;
        }

        protected async Task<BackendResponse> GetFromBackendWithBadRequestAsync(
            RestClient restClient,
            Func<RestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            ProblemDetailsHelper problemDetailsHelper,
            ICollection<KeyValuePair<string, string>>? headersToForward = null,
            CancellationToken cancellationToken = default)
        {
            var contentType = acceptType.ToMimeTypeString();

            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, contentType);
            if (headersToForward != null && headersToForward.Any())
            {
                backendRequest.AddHeaders(headersToForward);
            }

            var response = await ExecuteRequestAsync(restClient, backendRequest, cancellationToken);

            var downstreamVersion = response
                .Headers
                ?.FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase));

            if (response.StatusCode is HttpStatusCode.BadRequest)
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

            if (response.IsSuccessful)
            {
                return new BackendResponse(
                    response.Content,
                    downstreamVersion?.Value.ToString(),
                    DateTimeOffset.UtcNow,
                    response.ContentType == AcceptTypes.JsonLd ? response.ContentType : acceptType.ToMimeTypeString(),
                    false,
                    response.HeadersToKeyValuePairs(),
                    response.StatusCode);
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiException("Fout bij de bron.", (int)response.StatusCode, response.ErrorException);
        }

        protected async Task<IBackendResponse> GetFromBackendWithBadRequestAsync(
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

        private string GetPublicContentValue(RestResponse response, ProblemDetailsHelper helper)
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

        protected async Task<BackendResponse> GetFromBackendAsync(
            RestClient restClient,
            Func<RestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, acceptType.ToMimeTypeString());

            var response = await ExecuteRequestAsync(restClient, backendRequest, cancellationToken);

            if (response.IsSuccessful && response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created)
            {
                var downstreamVersion = response
                    .Headers?
                    .FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase));

                //if application json ld
                    // contenttype json ld
                // else
                   // accepttype.tomimestring

                return new BackendResponse(
                    response.Content,
                    downstreamVersion?.Value?.ToString(),
                    DateTimeOffset.UtcNow,
                    response.ContentType == AcceptTypes.JsonLd ? response.ContentType : acceptType.ToMimeTypeString(),
                    false,
                    response.HeadersToKeyValuePairs(),
                    response.StatusCode);
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiProblemDetailsException("Fout bij de bron.", (int)response.StatusCode, response.GetProblemDetails(), response.ErrorException);
        }

        private async Task<RestResponse> ExecuteRequestAsync(
            RestClient restClient,
            RestRequest backendRequest,
            CancellationToken cancellationToken)
        {
            AddApiKeyTokenHeaderFromRequest(_httpContextAccessor.HttpContext?.Request, backendRequest);
            var response = await restClient.ExecuteAsync(backendRequest, cancellationToken);

            // Api gateway hard limit: https://docs.aws.amazon.com/apigateway/latest/developerguide/limits.html
            if (response.Content?.Length > 10_000_000)
            {
                throw new ApiException("Response is te groot, probeer de 'limit' parameter te verkleinen en probeer opnieuw.", StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        private static void AddApiKeyTokenHeaderFromRequest(HttpRequest? currentRequest, RestRequest targetRequest)
        {
            if (currentRequest is null)
            {
                return;
            }

            var copyHeaders = new[] { ApiKeyAuthAttribute.ApiTokenHeaderName, ApiKeyAuthAttribute.ApiKeyHeaderName };
            var headersToCopy = currentRequest.Headers.Where(x => copyHeaders.Contains(x.Key));

            foreach (var (key, value) in headersToCopy)
            {
                targetRequest.AddHeader(key, value);
            }
        }
    }
}
