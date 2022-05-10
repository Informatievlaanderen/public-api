namespace Public.Api.Address.BackOffice
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
    using Version = Infrastructure.Version.Version;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Adressen")]
    [ApiOrder(Order = ApiOrder.Address)]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class AddressBackOfficeController : RegistryApiController<AddressBackOfficeController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand adres.";
        protected override string GoneExceptionMessage => "Verwijderd adres.";

        public AddressBackOfficeController(
            [KeyFilter(RegistryKeys.AddressBackOffice)] IRestClient restClient,
            [KeyFilter(RegistryKeys.AddressBackOffice)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<AddressBackOfficeController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);

    }
}
