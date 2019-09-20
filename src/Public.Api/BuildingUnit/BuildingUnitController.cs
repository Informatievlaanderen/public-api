namespace Public.Api.BuildingUnit
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouweenheden")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
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
    }
}
