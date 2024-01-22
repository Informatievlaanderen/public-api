namespace Public.Api.SuspiciousCases
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "VerdachteGevallen")]
    [ApiProduces(EndpointType.Oslo)] //TODO: what should this be
    public partial class SuspiciousCasesController : RegistryApiController<SuspiciousCasesController>
    {
        public SuspiciousCasesController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.SuspiciousCases)] IRestClient restClient,
            [KeyFilter(RegistryKeys.SuspiciousCases)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis, // TODO: can this be removed
            ILogger<SuspiciousCasesController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Oslo, context); //TODO: what should this be

        protected override string GoneExceptionMessage { get; }
        protected override string NotFoundExceptionMessage { get; }
    }
}
