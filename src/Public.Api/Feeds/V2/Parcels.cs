namespace Public.Api.Feeds.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using ParcelRegistry.Api.Legacy.Parcel.Responses;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Configuration;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class FeedV2Controller
    {
        /// <summary>
        /// Vraag een lijst met wijzigingen op percelen op in het XML of Atom formaat (v2).
        /// </summary>
        /// <param name="actionContextAccessor"></param>
        /// <param name="restClients"></param>
        /// <param name="responseOptions"></param>
        /// <param name="from">Eventidentificator (volgnummer) vanaf waar de feed moet gestart of hernomen worden (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="embed">Keuze welke info in het <![CDATA[&lt;Content&gt;]]>-gedeelte van de output moet zitten: "event", "object", "event,object" (optioneel).</param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met wijzigingen op percelen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als er geen API key is meegegeven.</response>
        /// <response code="403">Als u niet de correcte rechten heeft.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen", Name = nameof(GetParcelsFeedV2))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelSyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultFeedCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> GetParcelsFeedV2(
                 [FromServices] IActionContextAccessor actionContextAccessor,
                 [FromServices] IIndex<string, Lazy<IRestClient>> restClients,
                 [FromServices] IOptions<ParcelOptions> responseOptions,
                 [FromQuery] long? from,
                 [FromQuery] int? limit,
                 [FromQuery] string embed,
                 [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
                 CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendSyndicationRequest(
                "percelen",
                from,
                limit,
                embed);

            var value = await GetFromBackendAsync(
                restClients[RegistryKeys.Parcel].Value,
                BackendRequest,
                contentFormat.ContentType,
                HandleBadRequest,
                cancellationToken);

            return BackendListResponseResult.Create(
                value,
                Request.Query,
                responseOptions.Value.Syndication.NextUri,
                responseOptions.Value.Syndication.GetNextUri(actionContextAccessor));
        }
    }
}
