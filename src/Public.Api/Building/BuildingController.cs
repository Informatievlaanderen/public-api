namespace Public.Api.Building
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
    [ApiExplorerSettings(GroupName = "Gebouwen")]
    [ApiOrder(Order = ApiOrder.Building)]
    [Produces(AcceptTypes.Json/*, AcceptTypes.JsonLd, AcceptTypes.Xml*/)]
    public partial class BuildingController : RegistryApiController<BuildingController>
    {
        private const string Registry = "BuildingRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaand gebouw.";
        protected override string GoneExceptionMessage => "Verwijderd gebouw.";

        public BuildingController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
