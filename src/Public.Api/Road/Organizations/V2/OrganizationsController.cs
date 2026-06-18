namespace Public.Api.Road.Organizations.V2
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
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Wegen")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    [ApiOrder(ApiOrder.Road.Organization)]
    public partial class OrganizationsControllerV2 : RoadRegistryApiController<OrganizationsControllerV2>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande organisatie.";
        protected override string GoneExceptionMessage => "Verwijderde organisatie.";

        public OrganizationsControllerV2(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Road)] RestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<OrganizationsControllerV2> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
        }

        private ContentFormat DetermineFormat()
        {
            return ContentFormat.For(EndpointType.BackOffice, HttpContextAccessor.HttpContext);
        }
    }
}
