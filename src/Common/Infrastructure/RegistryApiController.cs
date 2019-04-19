namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http.Headers;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using RestSharp;

    public abstract class RegistryApiController<T> : ApiController<T>
    {
        private readonly IRestClient _restClient;

        protected readonly IFeatureToggle CacheToggle;

        protected RegistryApiController(
            IRestClient restClient,
            IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<T> logger) : base(redis, logger)
        {
            _restClient = restClient;
            CacheToggle = cacheToggle;
        }

        protected async Task<BackendResponse> GetFromCacheThenFromBackendAsync(
            string format,
            Func<RestRequest> createBackendRequestFunc,
            string cacheKey,
            RequestHeaders requestHeaders,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
            => await GetFromCacheThenFromBackendAsync(
                format,
                _restClient,
                createBackendRequestFunc,
                cacheKey,
                requestHeaders,
                handleNotOkResponseAction,
                cancellationToken);

        protected async Task<BackendResponse> GetFromBackendAsync(
            string format,
            Func<RestRequest> createBackendRequestFunc,
            RequestHeaders requestHeaders,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
            => await GetFromBackendAsync(
                format,
                _restClient,
                createBackendRequestFunc,
                requestHeaders,
                handleNotOkResponseAction,
                cancellationToken);

        protected string CreateCacheKeyForRequestQuery(string keyBaseValue)
        {
            if (string.IsNullOrWhiteSpace(keyBaseValue))
                throw new ArgumentNullException(nameof(keyBaseValue));

            bool ParameterHasValue(KeyValuePair<string, StringValues> parameter)
                => !string.IsNullOrWhiteSpace(parameter.Value.ToString());

            return Request.Query.Count == 0
                ? keyBaseValue
                : Request
                    .Query
                    .Where(ParameterHasValue)
                    .OrderBy(queryParameter => queryParameter.Key)
                    .Aggregate(keyBaseValue, (key, queryParameter) => $"{key}-({queryParameter.Key}:{queryParameter.Value})")
                    .Replace(":-(", ":(");
        }
    }
}
