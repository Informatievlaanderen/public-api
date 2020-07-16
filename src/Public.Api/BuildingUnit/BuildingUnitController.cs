namespace Public.Api.BuildingUnit
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouweenheden")]
    [ApiOrder(Order = ApiOrder.BuildingUnit)]
    [ApiProduces]
    public partial class BuildingUnitController : RegistryApiController<BuildingUnitController>
    {
        private const string Registry = "BuildingRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande gebouweenheid.";
        protected override string GoneExceptionMessage => "Verwijderde gebouweenheid.";

        public BuildingUnitController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingUnitController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
