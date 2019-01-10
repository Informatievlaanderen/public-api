namespace Public.Api.Municipality
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Autofac.Features.AttributeFilters;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class MunicipalityController : RegistryApiController<MunicipalityController>
    {
        private const string Registry = "MunicipalityRegistry";

        public MunicipalityController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<MunicipalityController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
