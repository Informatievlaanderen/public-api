namespace Public.Api.Road.Downloads.V2
{
    using System.Net.Http;
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;

    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Download")]
    [ApiOrder(ApiOrder.Road.Download)]
    [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
    public partial class DownloadControllerV2 : RoadRegistryApiController<DownloadControllerV2>
    {
        private readonly HttpClient _httpClient;
        protected override string NotFoundExceptionMessage => "Onbestaande download.";
        protected override string GoneExceptionMessage => "Verwijderde download.";

        public DownloadControllerV2(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<DownloadControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
            _httpClient = httpClient;
        }
    }
}
