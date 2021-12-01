namespace Public.Api.Municipality.Oslo
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
    [ApiExplorerSettings(GroupName = "Gemeenten OSLO")]
    [ApiOrder(Order = ApiOrder.Municipality)]
    [ApiProduces(EndpointType.Oslo)]
    public partial class MunicipalityOsloController : RegistryApiController<MunicipalityOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande gemeente.";
        protected override string GoneExceptionMessage => "Verwijderde gemeente.";

        public MunicipalityOsloController(
            [KeyFilter(RegistryKeys.MunicipalityV2)] IRestClient restClient,
            [KeyFilter(RegistryKeys.MunicipalityV2)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<MunicipalityOsloController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
