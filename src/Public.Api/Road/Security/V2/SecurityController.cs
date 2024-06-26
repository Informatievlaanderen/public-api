namespace Public.Api.Road.Security.V2
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

    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Security")]
    [ApiOrder(ApiOrder.Road.RoadUpload)]
    public partial class SecurityControllerV2 : RoadRegistryApiController<SecurityControllerV2>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande security.";
        protected override string GoneExceptionMessage => "Verwijderde security.";

        public SecurityControllerV2(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<SecurityControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, ActionContextAccessor.ActionContext);
        }
    }
}
