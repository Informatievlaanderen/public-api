namespace Public.Api.Municipality.V3
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V2, Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    [ApiProduces(EndpointType.Oslo)]
    public partial class MunicipalityOsloController : RegistryApiController<MunicipalityOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande gemeente.";
        protected override string GoneExceptionMessage => "Verwijderde gemeente.";

        public MunicipalityOsloController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.MunicipalityV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.MunicipalityV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<MunicipalityOsloController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(HttpContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
