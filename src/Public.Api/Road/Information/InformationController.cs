namespace Public.Api.Road.Information
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Informatie")]
    [ApiOrder(ApiOrder.Road.Information)]
    [ApiKeyAuth("Road")]
    public partial class InformationController : RegistryApiController<InformationController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande informatie.";
        protected override string GoneExceptionMessage => "Verwijderde informatie.";

        public InformationController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<InformationController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }
    }
}
