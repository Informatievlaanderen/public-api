namespace Public.Api.Road.Extracts
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
    [ApiExplorerSettings(GroupName = "Extract")]
    [ApiOrder(ApiOrder.Road.RoadExtract)]
    public partial class ExtractController : RegistryApiController<ExtractController>
    {
        private readonly HttpClient _httpClient;
        protected override string NotFoundExceptionMessage => "Onbestaand extract.";
        protected override string GoneExceptionMessage => "Verwijderd extract.";

        public ExtractController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
            _httpClient = httpClient;
        }
    }
}
