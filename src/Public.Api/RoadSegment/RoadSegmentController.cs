namespace Public.Api.RoadSegment
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiVisible]
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Wegsegmenten")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    [ApiOrder(ApiOrder.Road.RoadSegment)]
    public partial class RoadSegmentController : RegistryApiController<RoadSegmentController>
    {
        public RoadSegmentController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<RoadSegmentController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }

        protected override string NotFoundExceptionMessage => "Onbestaand wegsegment.";
        protected override string GoneExceptionMessage => "Verwijderd wegsegment.";

        private static ContentFormat DetermineFormat(ActionContext? context)
        {
            return ContentFormat.For(EndpointType.BackOffice, context);
        }
    }
}
