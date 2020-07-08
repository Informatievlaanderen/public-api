namespace Public.Api.PublicService
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Dienstverleningen")]
    [ApiOrder(Order = ApiOrder.PublicService)]
    [ApiProduces]
    public partial class PublicServiceController : RegistryApiController<PublicServiceController>
    {
        private const string Registry = "PublicServiceRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande dienstverlening.";
        protected override string GoneExceptionMessage => "Verwijderde dienstverlening.";

        public PublicServiceController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PublicServiceController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
