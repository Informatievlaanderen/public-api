namespace Public.Api.StreetName
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
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class StreetNameController : RegistryApiController<StreetNameController>
    {
        private const string Registry = "StreetNameRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
        protected override string GoneExceptionMessage => "Straatnaam verwijderd";

        public StreetNameController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<StreetNameController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
