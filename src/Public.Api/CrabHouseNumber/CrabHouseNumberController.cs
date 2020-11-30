namespace Public.Api.CrabHouseNumber
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "CRAB Huisnummers")]
    [ApiOrder(Order = ApiOrder.CrabHouseNumber)]
    [ApiProduces]
    public partial class CrabHouseNumberController : RegistryApiController<CrabHouseNumberController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand adres.";
        protected override string GoneExceptionMessage => "Verwijderd adres.";

        public CrabHouseNumberController(
            [KeyFilter(RegistryKeys.Address)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Address)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<CrabHouseNumberController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
