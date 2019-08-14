namespace Common.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http.Headers;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using RestSharp;
    using StackExchange.Redis;

    public abstract class ApiController<T> : ApiController
    {
        private const string ValueKey = "value";
        private const string LastModifiedKey = "lastModified";

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
                var key = $"{cacheKey}.{acceptType.ToString()}".ToLowerInvariant();

                try
                {
                    var db = _redis.GetDatabase();

                    var cachedValue =
                        await db.HashGetAsync(
                            key,
                            ValueKey,
                            CommandFlags.PreferSlave);

                    var cachedLastModified =
                        await db.HashGetAsync(
                            key,
                            LastModifiedKey,
                            CommandFlags.PreferSlave);

                    if (cachedValue.HasValue)
                        return new BackendResponse(
                            cachedValue,
                            DateTimeOffset.ParseExact(
                                cachedLastModified,
                                "O",
                                CultureInfo.InvariantCulture),
                            acceptType.ToMimeTypeString(),
                            true);
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
                return new BackendResponse(response.Content, DateTimeOffset.UtcNow, contentType, false);

            handleNotOkResponseAction(response.StatusCode);

            throw new ApiException("Fout bij de bron.", (int)response.StatusCode, response.ErrorException);
        }
    }
}
