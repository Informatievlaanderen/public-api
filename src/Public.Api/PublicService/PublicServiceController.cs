namespace Public.Api.PublicService
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
    [ApiExplorerSettings(GroupName = "Dienstverleningen")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class PublicServiceController : RegistryApiController<PublicServiceController>
    {
        private const string Registry = "PublicServiceRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande dienstverlening.";
        protected override string GoneExceptionMessage => "Dienstverlening verwijderd";

        public PublicServiceController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PublicServiceController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
