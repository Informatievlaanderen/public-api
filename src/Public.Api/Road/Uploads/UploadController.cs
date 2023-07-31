namespace Public.Api.Road.Uploads
{
    using System.Net.Http;
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
    [ApiExplorerSettings(GroupName = "Upload")]
    [ApiOrder(ApiOrder.Road.RoadUpload)]
    [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
    public partial class UploadController : RegistryApiController<UploadController>
    {
        private readonly HttpClient _httpClient;
        protected override string NotFoundExceptionMessage => "Onbestaande upload.";
        protected override string GoneExceptionMessage => "Verwijderde upload.";

        public UploadController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<UploadController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
            _httpClient = httpClient;
        }
    }
}
