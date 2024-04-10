namespace Public.Api.Road.Security
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion(Version.V1)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Security")]
    [ApiOrder(ApiOrder.Road.RoadUpload)]
    public partial class SecurityController : RoadRegistryApiController<SecurityController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande security.";
        protected override string GoneExceptionMessage => "Verwijderde security.";

        public SecurityController(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<SecurityController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, ActionContextAccessor.ActionContext);
        }
    }
}
