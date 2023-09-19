namespace Public.Api.Road
{
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Extensions;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using System.Net.Http;

    public abstract class RoadRegistryApiController<TController> : RegistryApiController<TController>
    {
        protected readonly IActionContextAccessor ActionContextAccessor;

        protected RoadRegistryApiController(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<TController> logger,
            IRestClient restClient,
            IFeatureToggle cacheToggle,
            IActionContextAccessor actionContextAccessor
        )
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
            ActionContextAccessor = actionContextAccessor;
        }

        protected RestRequest CreateBackendRestRequest(Method method, string path)
        {
            return new RestRequest(path)
                {
                    Method = method
                }
                .AddHeaderAuthorization(ActionContextAccessor);
        }

        protected HttpRequestMessage CreateBackendHttpRequestMessage(HttpMethod method, string path)
        {
            return new HttpRequestMessage(method, path)
                .AddHeaderAuthorization(ActionContextAccessor);
        }
    }
}
