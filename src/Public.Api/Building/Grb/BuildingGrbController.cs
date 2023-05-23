namespace Public.Api.Building.Grb
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

    [ApiVisible(false)]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouwen")]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class BuildingGrbController : RegistryApiController<BuildingGrbController>
    {
        protected override string GoneExceptionMessage { get; }
        protected override string NotFoundExceptionMessage { get; }

        public BuildingGrbController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.BuildingGrb)] IRestClient restClient,
            [KeyFilter(RegistryKeys.BuildingGrb)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingGrbController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);
    }
}
