namespace Public.Api.Road.Organizations
{
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

    [ApiVisible]
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Wegen")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    [ApiOrder(ApiOrder.Road.RoadSegment)]
    public partial class OrganizationsController : RegistryApiController<OrganizationsController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande organisatie.";
        protected override string GoneExceptionMessage => "Verwijderde organisatie.";

        public OrganizationsController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<OrganizationsController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }

        private static ContentFormat DetermineFormat(ActionContext? context)
        {
            return ContentFormat.For(EndpointType.BackOffice, context);
        }
    }
}
