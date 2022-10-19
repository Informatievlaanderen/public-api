namespace Public.Api.Tickets
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using RestSharp;
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
            [KeyFilter(RegistryKeys.TicketingService)] RestClient restClient,
            [KeyFilter(RegistryKeys.TicketingService)] IFeatureToggle cacheToggle,
            TicketingToggle ticketingToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<TicketingServiceController> logger)
            : base(restClient, cacheToggle, redis, logger)
        {
            _ticketingToggle = ticketingToggle;
        }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
