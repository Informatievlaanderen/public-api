namespace Public.Api.CrabSubaddress
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
    [ApiExplorerSettings(GroupName = "CRAB Subadressen")]
    [ApiOrder(Order = ApiOrder.CrabSubaddress)]
    [ApiProduces]
    public partial class CrabSubaddressController : RegistryApiController<CrabSubaddressController>
    {
        private const string Registry = "AddressRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaand adres.";
        protected override string GoneExceptionMessage => "Verwijderd adres.";

        public CrabSubaddressController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<CrabSubaddressController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
