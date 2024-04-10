namespace Public.Api.Road.Extracts.V2
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

    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Extract")]
    [ApiOrder(ApiOrder.Road.RoadExtract)]
    public partial class ExtractControllerV2 : RoadRegistryApiController<ExtractControllerV2>
    {
        private readonly HttpClient _httpClient;
        protected override string NotFoundExceptionMessage => "Onbestaand extract.";
        protected override string GoneExceptionMessage => "Verwijderd extract.";

        public ExtractControllerV2(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
            _httpClient = httpClient;
        }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);
    }
}
