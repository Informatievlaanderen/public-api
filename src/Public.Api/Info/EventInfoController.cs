namespace Public.Api.Info
{
    using System.Collections.Generic;
    using System.Threading;
    using Asp.Versioning;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Documentation;
    using Common.Infrastructure.Controllers;
    using Infrastructure.Configuration;
    using Infrastructure.ModelBinding;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    //[ApiVersion(Version.Current)]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("info/events")]
    [ApiExplorerSettings(GroupName = "Info", IgnoreApi = true)]
    [ApiOrder(ApiOrder.Status)]
    [Produces(AcceptTypes.Json)]
    public class EventInfoController : PublicApiController
    {
        /// <summary>
        /// Vraag de markdown documentatie van alle events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie van alle events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetAllEventsMarkdown(
            [FromServices] IEnumerable<IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators.GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor gemeenten events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor gemeenten events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gemeenten")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetMunicipalityEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.MunicipalityV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor postinfo events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor postinfo events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetPostalEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.PostalV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor straatnamen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor straatnamen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetStreetNameEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.StreetNameV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor adressen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor adressen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetAddressEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.AddressV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor gebouwen en gebouweenheden events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor gebouwen en gebouweenheden events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen")]
        [HttpGet("gebouweenheden")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetBuildingEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.BuildingV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor percelen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor percelen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetParcelEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.ParcelV2].GenerateFor(eventTags));

        /// <summary>
        /// Vraag de markdown documentatie voor wegen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="eventTags"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor wegen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("wegen")]
        [HttpGet("wegsegmenten")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetRoadEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            [FromQuery(Name = "tags"), EventTagArrayBinder] IEnumerable<EventTag> eventTags,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.Road].GenerateFor(eventTags));
    }
}
