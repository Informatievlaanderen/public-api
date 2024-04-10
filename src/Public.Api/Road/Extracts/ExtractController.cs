namespace Public.Api.Road.Extracts
{
    using System.Net.Http;
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
    [ApiExplorerSettings(GroupName = "Extract")]
    [ApiOrder(ApiOrder.Road.RoadExtract)]
    public partial class ExtractController : RoadRegistryApiController<ExtractController>
    {
        private readonly HttpClient _httpClient;
        protected override string NotFoundExceptionMessage => "Onbestaand extract.";
        protected override string GoneExceptionMessage => "Verwijderd extract.";

        public ExtractController(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
            _httpClient = httpClient;
        }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);
    }
}
