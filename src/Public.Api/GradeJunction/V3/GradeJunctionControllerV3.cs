namespace Public.Api.GradeJunction.V3
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
    [ApiOrder(ApiOrder.Road.GradeJunction.Root)]
    public partial class GradeJunctionControllerV3 : RoadRegistryApiController<GradeJunctionControllerV3>
    {
        public GradeJunctionControllerV3(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.RoadV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.RoadV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<GradeJunctionControllerV3> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }

        protected override string NotFoundExceptionMessage => "Onbestaand gelijkgrondse kruising.";
        protected override string GoneExceptionMessage => "Verwijderd gelijkgrondse kruising.";

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, ActionContextAccessor.ActionContext);
        }
    }
}
