namespace Public.Api.Road
{
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Extensions;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using System.Net.Http;

    public abstract class RoadRegistryApiController<TController> : RegistryApiController<TController>
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;

        protected RoadRegistryApiController(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<TController> logger,
            RestClient restClient,
            IFeatureToggle cacheToggle
        )
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected RestRequest CreateBackendRestRequest(Method method, string path)
        {
            return new RestRequest(path, method)
                .AddHeaderAuthorization(HttpContextAccessor);
        }

        protected HttpRequestMessage CreateBackendHttpRequestMessage(HttpMethod method, string path)
        {
            return new HttpRequestMessage(method, path)
                .AddHeaderAuthorization(HttpContextAccessor);
        }
    }
}
