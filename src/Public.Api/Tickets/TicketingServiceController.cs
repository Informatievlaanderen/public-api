namespace Public.Api.Tickets
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Version = Infrastructure.Version.Version;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Tickets")]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class TicketingServiceController : RegistryApiController<TicketingServiceController>
    {
        private readonly TicketingToggle _ticketingToggle;

        protected override string NotFoundExceptionMessage => "Onbestaand ticket.";
        protected override string GoneExceptionMessage => "Verwijderd ticket.";

        public TicketingServiceController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.TicketingService)] IRestClient restClient,
            [KeyFilter(RegistryKeys.TicketingService)] IFeatureToggle cacheToggle,
            TicketingToggle ticketingToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<TicketingServiceController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
        {
            _ticketingToggle = ticketingToggle;
        }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
