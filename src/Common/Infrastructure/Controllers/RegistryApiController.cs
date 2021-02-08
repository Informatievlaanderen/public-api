namespace Common.Infrastructure.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using RestSharp;

    public abstract class RegistryApiController<T> : ApiController<T>
    {
        private readonly IRestClient _restClient;

        protected readonly IFeatureToggle CacheToggle;

        protected abstract string GoneExceptionMessage { get; }
        protected abstract string NotFoundExceptionMessage { get; }

        protected const int DefaultDetailCaching = 24 * 60 * 60; // Hours, Minutes, Second
        protected const int DefaultListCaching = 0;
        protected const int DefaultCountCaching = 0;

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
            AcceptType acceptType,
            Func<IRestRequest> createBackendRequestFunc,
            string cacheKey,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
            => await GetFromCacheThenFromBackendAsync(
                acceptType,
                _restClient,
                createBackendRequestFunc,
                cacheKey,
                handleNotOkResponseAction,
                cancellationToken);

        protected async Task<BackendResponse> GetFromBackendAsync(
            AcceptType acceptType,
            Func<IRestRequest> createBackendRequestFunc,
            Action<HttpStatusCode> handleNotOkResponseAction,
            CancellationToken cancellationToken)
            => await GetFromBackendAsync(
                _restClient,
                createBackendRequestFunc,
                acceptType,
                handleNotOkResponseAction,
                cancellationToken);

        protected async Task<BackendResponse> GetFromBackendWithBadRequestAsync(
            AcceptType acceptType,
            Func<IRestRequest> createBackendRequestFunc,
            Action<HttpStatusCode> handleNotOkResponseAction,
            ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
            => await GetFromBackendWithBadRequestAsync(
                _restClient,
                createBackendRequestFunc,
                acceptType,
                handleNotOkResponseAction,
                problemDetailsHelper,
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

        protected Action<HttpStatusCode> CreateDefaultHandleBadRequest() =>
            CreateHandleBadRequest(
                goneExceptionMessage: GoneExceptionMessage,
                notFoundExceptionMessage: NotFoundExceptionMessage);

        protected Action<HttpStatusCode> CreateHandleBadRequest(
            string notAcceptableExceptionMessage = null,
            string badRequestExceptionMessage = null,
            string goneExceptionMessage = null,
            string notFoundExceptionMessage = null) =>
            httpStatusCode => HandleBadRequest(
                httpStatusCode,
                notAcceptableExceptionMessage,
                badRequestExceptionMessage,
                goneExceptionMessage,
                notFoundExceptionMessage);

        private void HandleBadRequest(
            HttpStatusCode statusCode,
            string notAcceptableExceptionMessage,
            string badRequestExceptionMessage,
            string goneExceptionMessage,
            string notFoundExceptionMessage)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotAcceptable:
                    throw new ApiException(string.IsNullOrWhiteSpace(notAcceptableExceptionMessage)
                        ? "Ongeldig formaat."
                        : notAcceptableExceptionMessage, StatusCodes.Status406NotAcceptable);

                case HttpStatusCode.BadRequest:
                    throw new ApiException(string.IsNullOrWhiteSpace(badRequestExceptionMessage)
                        ? "Ongeldige vraag."
                        : badRequestExceptionMessage, StatusCodes.Status400BadRequest);

                case HttpStatusCode.Gone:
                    throw new ApiException(string.IsNullOrWhiteSpace(goneExceptionMessage)
                        ? "Verwijderd."
                        : GoneExceptionMessage, StatusCodes.Status410Gone);

                case HttpStatusCode.NotFound:
                    throw new ApiException(string.IsNullOrWhiteSpace(notFoundExceptionMessage)
                        ? "Niet gevonden."
                        : notFoundExceptionMessage, StatusCodes.Status404NotFound);
            }
        }
    }
}
