namespace Public.Api.Road.Inwinning
{
    using System.Net.Http;
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Inwinning")]
    [ApiOrder(ApiOrder.Road.RoadInwinning)]
    public partial class InwinningControllerV2 : RoadRegistryApiController<InwinningControllerV2>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand extract.";
        protected override string GoneExceptionMessage => "Verwijderd extract.";

        public InwinningControllerV2(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] HttpClient httpClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<InwinningControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }
    }
}
