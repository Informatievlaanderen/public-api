namespace Public.Api.BuildingUnit.V3
{
    using Asp.Versioning;
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
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V2, Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouweenheden")]
    [ApiProduces(EndpointType.Oslo)]
    public partial class BuildingUnitOsloController : RegistryApiController<BuildingUnitOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande gebouweenheid.";
        protected override string GoneExceptionMessage => "Verwijderde gebouweenheid.";

        public BuildingUnitOsloController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.BuildingV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.BuildingV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingUnitOsloController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(HttpContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
