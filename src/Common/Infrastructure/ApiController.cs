namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Infrastructure;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Headers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RestSharp;
    using StackExchange.Redis;

    [ApiController]
    public abstract class ApiController<T> : ApiController
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

        protected static string DetermineAndSetFormat(
            string format,
            IActionContextAccessor actionContextAccessor,
            HttpRequest request)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            var acceptType = request.GetTypedHeaders().DetermineAcceptType(format);
            var contentType = acceptType.ToMimeTypeString();

            request.Headers[HeaderNames.Accept] = contentType;

            return format;
        }

        protected async Task<BackendResponse> GetFromCacheThenFromBackendAsync(
            string format,
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            string cacheKey,
            RequestHeaders requestHeaders,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            var acceptType = requestHeaders.DetermineAcceptType(format);

            if (_redis != null)
            {
                var key = $"{cacheKey}.{acceptType}".ToLowerInvariant();

                try
                {
                    var db = _redis.GetDatabase();

                    var cachedValues =
                        await db.HashGetAllAsync(
                            key,
                            CommandFlags.PreferSlave);

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
                                true);
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

        protected async Task<BackendResponse> GetFromBackendAsync(
            string format,
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            RequestHeaders requestHeaders,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
            => await GetFromBackendAsync(
                restClient,
                createBackendRequestFunc,
                requestHeaders.DetermineAcceptType(format),
                handleNotOkResponseAction,
                cancellationToken);

        protected static async Task<BackendResponse> GetFromBackendWithBadRequestAsync(
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            var contentType = acceptType.ToMimeTypeString();

            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, contentType);

            var response = await restClient.ExecuteTaskAsync(backendRequest, cancellationToken);

            if ((response.IsSuccessful && response.StatusCode == HttpStatusCode.OK) || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var downstreamVersion = response
                    .Headers
                    .FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase));

                var responseContentType = response.StatusCode == HttpStatusCode.OK
                    ? contentType
                    : response.ContentType;

                return new BackendResponse(
                    response.Content,
                    downstreamVersion?.Value.ToString(),
                    DateTimeOffset.UtcNow,
                    responseContentType,
                    false,
                    response.StatusCode);
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiException("Fout bij de bron.", (int)response.StatusCode, response.ErrorException);
        }

        private static async Task<BackendResponse> GetFromBackendAsync(
            IRestClient restClient,
            Func<IRestRequest> createBackendRequestFunc,
            AcceptType acceptType,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
        {
            var contentType = acceptType.ToMimeTypeString();

            var backendRequest = createBackendRequestFunc();
            backendRequest.AddHeader(HeaderNames.Accept, contentType);

            var response = await restClient.ExecuteTaskAsync(backendRequest, cancellationToken);

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
                    false);
            }

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiException("Fout bij de bron.", (int)response.StatusCode, response.ErrorException);
        }
    }
}
