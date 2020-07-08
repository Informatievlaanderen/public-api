namespace Public.Api.CrabBuilding
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "CRAB Gebouwen")]
    [ApiOrder(Order = ApiOrder.CrabBuildings)]
    [ApiProduces]
    public partial class CrabBuildingController : RegistryApiController<CrabBuildingController>
    {
        private const string Registry = "BuildingRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaand gebouw.";
        protected override string GoneExceptionMessage => "Verwijderd gebouw.";

        public CrabBuildingController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<CrabBuildingController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
