namespace Public.Api.Road.Changes
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Activiteit")]
    [ApiOrder(Order = ApiOrder.RoadChangeFeed)]
    public partial class ChangeFeedController : RegistryApiController<ChangeFeedController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande activiteit.";
        protected override string GoneExceptionMessage => "Verwijderde activiteit.";

        public ChangeFeedController(
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ChangeFeedController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
