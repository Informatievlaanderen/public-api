namespace Public.Api.RoadSegment.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Infrastructure.Options;
    using RoadRegistry.BackOffice.Api.V2.RoadSegments;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentControllerV3
    {
        private const string ListRoadSegmentsRoute = "wegsegmenten";

        /// <summary>
        ///     Vraag een lijst met wegsegmenten op (v3).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt. De offset is echter beperkt tot 1000000, indien meer data dient ingelezen te worden is het gebruik van extra filters aangewezen op de service of verwijzen we naar de <a href="https://vlaamseoverheid.atlassian.net/wiki/x/LYay3QE" target="_blank" >downloadproducten van het gebouwen- en adressenregister</a> (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="straatnaamObjectId">Filter op de objectidentificator van de gekoppelde straatnaam (exact) (optioneel).</param>
        /// <param name="wegsegmentstatus">
        /// Filter op de status van het wegsegment (exact) (optioneel). \
        /// `"gepland"` `"gerealiseerd"` `"gehistoreerd"` `"niet gerealiseerd"` `"buiten gebruik"`
        /// </param>
        /// <param name="apiOptions"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met wegsegmenten gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet(ListRoadSegmentsRoute, Name = nameof(ListRoadSegmentsV3))]
        [ApiOrder(ApiOrder.Road.RoadSegment.List)]
        [ProducesResponseType(typeof(WegsegmentV2Lijst), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WegsegmentV2LijstResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerOperation(OperationId = nameof(ListRoadSegmentsV3))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListRoadSegmentsV3(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] int? straatnaamObjectId,
            [FromQuery] string wegsegmentstatus,
            [FromServices] ApiOptions apiOptions,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ListRoadSegmentsV3Toggle featureToggle,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, ListRoadSegmentsRoute)
                    .AddPagination(offset, limit)
                    .AddFiltering(new RoadSegmentFilter
                    {
                        StreetNameId = straatnaamObjectId?.ToString(),
                        Status = wegsegmentstatus
                    });

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            // The road registry links the next page on the base url of this API, the way it links the detail of a
            // road segment; the filters asked for here are added to it.
            return BackendListResponseResult.Create(
                value,
                Request.Query,
                $"{apiOptions.BaseUrl}/v3/{ListRoadSegmentsRoute}?offset={{0}}&limit={{1}}");
        }
    }
}
