namespace Public.Api.StreetName.BackOffice
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
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    [ApiOrder(Order = ApiOrder.StreetName)]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class StreetNameBackOfficeController : RegistryApiController<StreetNameBackOfficeController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
        protected override string GoneExceptionMessage => "Verwijderde straatnaam.";

        public StreetNameBackOfficeController(
            [KeyFilter(RegistryKeys.StreetNameBackOffice)] IRestClient restClient,
            [KeyFilter(RegistryKeys.StreetNameBackOffice)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<StreetNameBackOfficeController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);

    }
}
