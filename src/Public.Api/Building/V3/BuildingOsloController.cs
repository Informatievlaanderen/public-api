namespace Public.Api.Building.V3
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V2,  Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouwen")]
    [ApiProduces(EndpointType.Oslo)]
    public partial class BuildingOsloController : RegistryApiController<BuildingOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand gebouw.";
        protected override string GoneExceptionMessage => "Verwijderd gebouw.";

        public BuildingOsloController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.BuildingV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.BuildingV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingOsloController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(HttpContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
