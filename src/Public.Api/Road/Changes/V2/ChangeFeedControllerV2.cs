namespace Public.Api.Road.Changes.V2
{
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

    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Activiteit")]
    [ApiOrder(ApiOrder.Road.ChangeFeed)]
    public partial class ChangeFeedControllerV2 : RoadRegistryApiController<ChangeFeedControllerV2>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande activiteit.";
        protected override string GoneExceptionMessage => "Verwijderde activiteit.";

        public ChangeFeedControllerV2(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ChangeFeedControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor) { }
    }
}
