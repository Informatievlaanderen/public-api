namespace Public.Api.Road.Changes
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

    [ApiVersion(Version.V1)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Activiteit")]
    [ApiOrder(ApiOrder.Road.ChangeFeed)]
    public partial class ChangeFeedController : RoadRegistryApiController<ChangeFeedController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande activiteit.";
        protected override string GoneExceptionMessage => "Verwijderde activiteit.";

        public ChangeFeedController(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ChangeFeedController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor) { }
    }
}
