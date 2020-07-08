namespace Public.Api.StreetName
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
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    [ApiOrder(Order = ApiOrder.StreetName)]
    [ApiProduces]
    public partial class StreetNameController : RegistryApiController<StreetNameController>
    {
        private const string Registry = "StreetNameRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
        protected override string GoneExceptionMessage => "Verwijderde straatnaam.";

        public StreetNameController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<StreetNameController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
