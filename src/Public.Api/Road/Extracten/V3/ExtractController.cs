namespace Public.Api.Road.Extracten.V3
{
    using System.Net.Http;
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Extracten")]
    [ApiOrder(ApiOrder.Road.RoadExtract)]
    public partial class ExtractControllerV3 : RoadRegistryApiController<ExtractControllerV3>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand extract.";
        protected override string GoneExceptionMessage => "Verwijderd extract.";

        public ExtractControllerV3(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.RoadV3)] RestClient restClient,
            [KeyFilter(RegistryKeys.RoadV3)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.RoadV3)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractControllerV3> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }
    }
}
