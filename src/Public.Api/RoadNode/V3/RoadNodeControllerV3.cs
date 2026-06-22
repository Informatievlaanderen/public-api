namespace Public.Api.RoadNode.V3
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using Road;

    [ApiVisible]
    [ApiVersion(Version.V3)]
    [AdvertiseApiVersions(Version.V3)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Wegen")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    [ApiOrder(ApiOrder.Road.RoadNode.Root)]
    public partial class RoadNodeControllerV3 : RoadRegistryApiController<RoadNodeControllerV3>
    {
        public RoadNodeControllerV3(
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<RoadNodeControllerV3> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle, actionContextAccessor)
        {
        }

        protected override string NotFoundExceptionMessage => "Onbestaand wegknoop.";
        protected override string GoneExceptionMessage => "Verwijderd wegknoop.";

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, ActionContextAccessor.ActionContext);
        }
    }
}
