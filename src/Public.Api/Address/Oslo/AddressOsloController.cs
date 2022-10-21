namespace Public.Api.Address.Oslo
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
    [ApiExplorerSettings(GroupName = "Adressen")]
    [ApiProduces(EndpointType.Oslo)]
    public partial class AddressOsloController : RegistryApiController<AddressOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand adres.";
        protected override string GoneExceptionMessage => "Verwijderd adres.";

        public AddressOsloController(
            [KeyFilter(RegistryKeys.AddressV2)] IRestClient restClient,
            [KeyFilter(RegistryKeys.AddressV2)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<AddressOsloController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
