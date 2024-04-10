namespace Public.Api.Building.BackOffice
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
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouwen")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class BuildingBackOfficeController : RegistryApiController<BuildingBackOfficeController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand gebouw.";
        protected override string GoneExceptionMessage => "Verwijderd gebouw.";

        public BuildingBackOfficeController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.BuildingBackOffice)]
            RestClient restClient,
            [KeyFilter(RegistryKeys.BuildingBackOffice)]
            IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingBackOfficeController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);
    }
}
