namespace Public.Api.TicketingService
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
    [ApiOrder(Order = ApiOrder.TicketingService)]
    [ApiProduces]
    public partial class TicketingServiceController : RegistryApiController<TicketingServiceController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand ticket.";
        protected override string GoneExceptionMessage => "Verwijderd ticket.";

        public TicketingServiceController(
            [KeyFilter(RegistryKeys.TicketingService)] IRestClient restClient,
            [KeyFilter(RegistryKeys.TicketingService)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<TicketingServiceController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
