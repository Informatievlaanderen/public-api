namespace Public.Api.Info
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.EventHandling.Documentation;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("info/events")]
    [ApiExplorerSettings(GroupName = "Info", IgnoreApi = true)]
    [ApiOrder(Order = ApiOrder.Status)]
    [Produces(AcceptTypes.Json)]
    public class EventInfoController : PublicApiController
    {
        /// <summary>
        /// Vraag de markdown documentatie van alle events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie van alle events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetAllEventsMarkdown(
            [FromServices] IEnumerable<IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators.Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor gemeenten events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor gemeenten events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gemeenten")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetMunicipalityEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.Municipality].Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor postinfo events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor postinfo events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetPostalEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.Postal].Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor straatnamen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor straatnamen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetStreetNameEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.StreetName].Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor adressen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor adressen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetAddressEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.Address].Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor gebouwen en gebouweenheden events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor gebouwen en gebouweenheden events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen")]
        [HttpGet("gebouweenheden")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetBuildingEventsMarkdown(
            [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
            CancellationToken cancellationToken = default)
            => Content(markdownGenerators[RegistryKeys.Building].Generate());

        /// <summary>
        /// Vraag de markdown documentatie voor percelen events op.
        /// </summary>
        /// <param name="markdownGenerators"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de markdown documentatie voor percelen events gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen")]
            [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
            [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
            public IActionResult GetParcelEventsMarkdown(
                [FromServices] IIndex<string, IRegistryEventsMarkdownGenerator> markdownGenerators,
                CancellationToken cancellationToken = default)
                => Content(markdownGenerators[RegistryKeys.Parcel].Generate());
        }
}
