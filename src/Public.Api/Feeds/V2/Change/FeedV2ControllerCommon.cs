namespace Public.Api.Feeds.V2.Change
{
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using Asp.Versioning;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [ApiRoute("feeds/wijzigingen")]
    [ApiExplorerSettings(GroupName = FeedsGroupName)]
    [ApiOrder(ApiOrder.Feeds)]
    [ApiProduces(EndpointType.ChangeFeed)]
    [ApiKeyAuth("Sync")]
    public partial class ChangeFeedV2Controller : ApiController<ChangeFeedV2Controller>
    {
        private readonly IIndex<string, IFeatureToggle> _cacheToggles;

        private const int DefaultFeedCaching = 24 * 60 * 60; // Hours, Minutes, Second

        public const string FeedsGroupName = "Feeds";

        public ChangeFeedV2Controller(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            [FromServices] IIndex<string, IFeatureToggle> cacheToggles,
            ILogger<ChangeFeedV2Controller> logger)
            : base(httpContextAccessor, redis, logger)
        {
            _cacheToggles = cacheToggles;
        }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.ChangeFeed, context);

        private static RestRequest CreateBackendChangeFeedRequest(
            string resourcename,
            int? page,
            int? feedPosition)
            => new RestRequest($"{resourcename}/wijzigingen")
                    .AddFiltering(new
                    {
                        page = page,
                        feedPosition = feedPosition
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

        protected bool CanGetFromCache(string toggleName, ActionContext actionContext)
        {
            return _cacheToggles[toggleName].FeatureEnabled
                   && !actionContext.HttpContext.Request
                       .Headers
                       .CacheControl
                       .Any(x => CacheControlHeaderValue.TryParse(x, out var value) && value.NoCache);
        }
    }
}
