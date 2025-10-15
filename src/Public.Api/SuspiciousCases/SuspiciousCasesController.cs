namespace Public.Api.SuspiciousCases
{
    using Asp.Versioning;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Verdachte gevallen")]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class SuspiciousCasesController : RegistryApiController<SuspiciousCasesController>
    {
        public SuspiciousCasesController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.SuspiciousCases)] RestClient restClient,
            [KeyFilter(RegistryKeys.SuspiciousCases)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<SuspiciousCasesController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(ActionContext? context)
            => ContentFormat.For(EndpointType.BackOffice, context);

        protected override string GoneExceptionMessage { get; }
        protected override string NotFoundExceptionMessage { get; }
    }
}
