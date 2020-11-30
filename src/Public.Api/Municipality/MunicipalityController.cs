namespace Public.Api.Municipality
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
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    [ApiOrder(Order = ApiOrder.Municipality)]
    [ApiProduces]
    public partial class MunicipalityController : RegistryApiController<MunicipalityController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande gemeente.";
        protected override string GoneExceptionMessage => "Verwijderde gemeente.";

        public MunicipalityController(
            [KeyFilter(RegistryKeys.Municipality)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Municipality)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<MunicipalityController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
