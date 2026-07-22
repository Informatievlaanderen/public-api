namespace Public.Api.Feeds.V3.Change
{
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using Asp.Versioning;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [ApiRoute("feeds/wijzigingen")]
    [ApiExplorerSettings(GroupName = FeedsGroupName)]
    [ApiOrder(ApiOrder.Feeds)]
    [ApiProduces(EndpointType.ChangeFeed)]
    [ApiKeyAuth("Sync")]
    public partial class ChangeFeedV3Controller : ApiController<ChangeFeedV3Controller>
    {
        private readonly IIndex<string, IFeatureToggle> _cacheToggles;

        private const int DefaultFeedCaching = 24 * 60 * 60; // Hours, Minutes, Second

        public const string FeedsGroupName = "Feeds";

        public ChangeFeedV3Controller(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            [FromServices] IIndex<string, IFeatureToggle> cacheToggles,
            ILogger<ChangeFeedV3Controller> logger)
            : base(httpContextAccessor, redis, logger)
        {
            _cacheToggles = cacheToggles;
        }

        private static ContentFormat DetermineFormat(HttpContext context)
            => ContentFormat.For(EndpointType.ChangeFeed, context);

        private static RestRequest CreateBackendChangeFeedRequest(
            string resourcename,
            int? page)
            => new RestRequest($"{resourcename}/wijzigingen")
                    .AddFiltering(new
                    {
                        page = page,
                        //feedPosition = feedPosition
                    });

        protected void HandleBadRequest(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotAcceptable:
                    throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);

                case HttpStatusCode.BadRequest:
                    throw new ApiException("Ongeldige vraag.", StatusCodes.Status400BadRequest);
            }
        }

        protected bool CanGetFromCache(string toggleName, HttpContext httpContext)
        {
            return _cacheToggles[toggleName].FeatureEnabled
                   && !httpContext.Request
                       .Headers
                       .CacheControl
                       .Any(x => CacheControlHeaderValue.TryParse(x, out var value) && value.NoCache);
        }
    }
}
