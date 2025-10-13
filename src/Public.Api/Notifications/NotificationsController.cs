namespace Public.Api.Notifications
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
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using Public.Api.Infrastructure.Version;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Notificaties")]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class NotificationsController : RegistryApiController<NotificationsController>
    {
        protected const string BackOfficeVersion = "v1";

        public NotificationsController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Notifications)] RestClient restClient,
            [KeyFilter(RegistryKeys.Notifications)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<NotificationsController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(ActionContext? context)
            => ContentFormat.For(EndpointType.BackOffice, context);

        protected override string GoneExceptionMessage { get; }
        protected override string NotFoundExceptionMessage { get; }
    }
}
