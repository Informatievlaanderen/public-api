namespace Public.Api.Municipality
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
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    [ApiOrder(Order = ApiOrder.Municipality)]
    public partial class MunicipalityController : RegistryApiController<MunicipalityController>
    {
        private const string Registry = "MunicipalityRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande gemeente.";
        protected override string GoneExceptionMessage => "Verwijderde gemeente.";

        public MunicipalityController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<MunicipalityController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
