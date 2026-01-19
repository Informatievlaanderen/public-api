namespace Public.Api.Feeds.V2.Change
{
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using Asp.Versioning;
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
        public const string FeedsGroupName = "Feeds";

        protected const int DefaultFeedCaching = 0;

        public ChangeFeedV2Controller(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<ChangeFeedV2Controller> logger)
            : base(httpContextAccessor, redis, logger) { }

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

        protected bool CanGetFromCache(IFeatureToggle cacheToggle, ActionContext actionContext)
        {
            return cacheToggle.FeatureEnabled
                   && !actionContext.HttpContext.Request
                       .Headers
                       .CacheControl
                       .Any(x => CacheControlHeaderValue.TryParse(x, out var value) && value.NoCache);
        }
    }
}
