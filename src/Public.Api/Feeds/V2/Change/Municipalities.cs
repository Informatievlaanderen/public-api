namespace Public.Api.Feeds.V2.Change
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using CloudNative.CloudEvents;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using MunicipalityRegistry.Api.Oslo.Municipality.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class ChangeFeedV2Controller
    {
        /// <summary>
        /// Vraag een lijst op met wijzigingen over gemeenten (v2).
        /// </summary>
        /// <param name="actionContextAccessor"></param>
        /// <param name="restClients"></param>
        /// <param name="pagina">Paginanummer dat aangeeft vanaf welke pagina de feedresultaten worden opgehaald (optioneel).</param>
        /// <param name="changeFeedMunicipalityToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel).</param>
        /// <returns></returns>
        /// <response code="200">Als de opvraging van de lijst met wijzigingen over gemeenten gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als er geen API key is meegegeven.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gemeenten", Name = nameof(ChangeFeedMunicipality))]
        [ProducesResponseType(typeof(List<CloudEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-page-complete", "bool", "Geeft aan of de pagina definitief is.<br/>`true`: er worden geen nieuwe wijzigingen meer aan deze pagina toegevoegd.<br/>`false`: er kunnen nog wijzigingen aan deze pagina worden toegevoegd.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityFeedResultExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheExpiration(MaxAge = DefaultFeedCaching)]
        [SwaggerOperation(Description = "Vraag een lijst op van wijzigingen over gemeenten, bedoeld om een lokale kopie van het register efficiënt te synchroniseren.<br/>" +
                                        "De response bestaat uit een batch <b>CloudEvents</b>. Voor de betekenis van de standaard CloudEvents-attributen verwijzen we naar de officiële <a href=\"https://cloudevents.io/\" target=\"_blank\">CloudEvents-documentatie</a>.<br/>" +
                                        "Geometrieën worden altijd meegegeven als <b>GML</b>, inclusief het <b>SRS</b>. Raadpleeg dit SRS altijd bij verwerking, aangezien de projectie kan wijzigen.<br/>" +
                                        "Aanbeveling: vraag de feed niet vaker op dan nodig. Stem de opvraagfrequentie af op je verwerkingscapaciteit en de gewenste actualiteit van je lokale kopie.")]
        public async Task<IActionResult> ChangeFeedMunicipality(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IIndex<string, Lazy<RestClient>> restClients,
            [FromServices] ChangeFeedMunicipalityToggle changeFeedMunicipalityToggle,
            [FromQuery] int? pagina,
            //[FromQuery] int? feedPositie,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!changeFeedMunicipalityToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            //if (pagina is null && feedPositie is null)
            pagina ??= 1;
            var cacheKey = $"feed/municipality:{pagina}";

            RestRequest BackendRequest() => CreateBackendChangeFeedRequest(
                "gemeenten",
                pagina);

            var value = await (CanGetFromCache(RegistryKeys.MunicipalityV2, actionContextAccessor.ActionContext)
                ? GetFromCacheThenFromBackendAsync(
                    contentFormat.ContentType,
                    restClients[RegistryKeys.MunicipalityV2].Value,
                    BackendRequest,
                    cacheKey,
                    HandleBadRequest,
                    cancellationToken)
                : GetFromBackendAsync(
                    restClients[RegistryKeys.MunicipalityV2].Value,
                    BackendRequest,
                    contentFormat.ContentType,
                    HandleBadRequest,
                    cancellationToken));

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("gemeenten/{nisCode}", Name = nameof(ChangeFeedMunicipalityById))]
        [ProducesResponseType(typeof(List<CloudEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityFeedResultExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultFeedCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ChangeFeedMunicipalityById(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IIndex<string, Lazy<RestClient>> restClients,
            [FromRoute] string nisCode,
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromServices] ChangeFeedMunicipalityToggle changeFeedMunicipalityToggle,
            CancellationToken cancellationToken = default)
        {
            if (!changeFeedMunicipalityToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            var value = await GetFromBackendAsync(
                restClients[RegistryKeys.MunicipalityV2].Value,
                () => new RestRequest($"gemeenten/{nisCode}/wijzigingen", Method.Get).AddPagination(offset, limit),
                contentFormat.ContentType,
                HandleBadRequest,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value);
        }
    }
}
