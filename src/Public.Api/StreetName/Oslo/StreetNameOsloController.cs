namespace Public.Api.StreetName.Oslo
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    [ApiProduces(EndpointType.Oslo)]
    public partial class StreetNameOsloController : RegistryApiController<StreetNameOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
        protected override string GoneExceptionMessage => "Verwijderde straatnaam.";

        public StreetNameOsloController(
            [KeyFilter(RegistryKeys.StreetNameV2)] IRestClient restClient,
            [KeyFilter(RegistryKeys.StreetNameV2)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<StreetNameOsloController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
