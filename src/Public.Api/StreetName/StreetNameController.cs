namespace Public.Api.StreetName
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
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    [ApiOrder(Order = ApiOrder.StreetName)]
    [ApiProduces]
    public partial class StreetNameController : RegistryApiController<StreetNameController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
        protected override string GoneExceptionMessage => "Verwijderde straatnaam.";

        public StreetNameController(
            [KeyFilter(RegistryKeys.StreetName)] IRestClient restClient,
            [KeyFilter(RegistryKeys.StreetName)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<StreetNameController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
