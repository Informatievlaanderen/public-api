namespace Public.Api.Road.Security
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
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
    [ApiExplorerSettings(GroupName = "Security")]
    [ApiOrder(ApiOrder.Road.RoadUpload)]
    public partial class SecurityController : RegistryApiController<SecurityController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande security.";
        protected override string GoneExceptionMessage => "Verwijderde security.";

        public SecurityController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<SecurityController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }

        private static ContentFormat DetermineFormat(ActionContext? context)
        {
            return ContentFormat.For(EndpointType.BackOffice, context);
        }
    }
}
