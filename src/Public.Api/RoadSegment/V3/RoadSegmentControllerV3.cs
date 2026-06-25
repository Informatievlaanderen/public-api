namespace Public.Api.RoadSegment.V3
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using Road;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Wegen")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    [ApiOrder(ApiOrder.Road.RoadSegment.Root)]
    public partial class RoadSegmentControllerV3 : RoadRegistryApiController<RoadSegmentControllerV3>
    {
        public RoadSegmentControllerV3(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.RoadV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.RoadV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<RoadSegmentControllerV3> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }

        protected override string NotFoundExceptionMessage => "Onbestaand wegsegment.";
        protected override string GoneExceptionMessage => "Verwijderd wegsegment.";

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, ActionContextAccessor.ActionContext);
        }
    }
}
