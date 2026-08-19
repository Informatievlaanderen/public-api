namespace Public.Api.BuildingUnit.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.Oslo.BuildingUnit.V3.Detail;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingUnitOsloController
    {
        /// <summary>
        /// Vraag een gebouweenheid op (v3).
        /// </summary>
        /// <param name="objectId">Identificator van de gebouweenheid.</param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="osloV3BuildingUnitToggle"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de gebouweenheid gevonden is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als de gebouweenheid niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als de gebouweenheid verwijderd is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouweenheden/{objectId}", Name = nameof(GetBuildingUnitV3))]
        [ApiOrder(ApiOrder.BuildingUnit.V3 + 1)]
        [ProducesResponseType(typeof(BuildingUnitOsloV3Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingUnitOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BuildingUnitNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(BuildingUnitGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetBuildingUnitV3(
            [FromRoute] int objectId,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] OsloV3BuildingUnitToggle osloV3BuildingUnitToggle,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!osloV3BuildingUnitToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"oslo-v3/buildingunit:{objectId}";

            var value = await (CanGetFromCache(httpContextAccessor.HttpContext!)
                ? GetFromCacheThenFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    cacheKey,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken)
                : GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken));

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private static RestRequest CreateBackendDetailRequest(int buildingUnitId)
        {
            var request = new RestRequest("gebouweenheden/{buildingUnitId}");
            request.AddParameter("buildingUnitId", buildingUnitId, ParameterType.UrlSegment);
            return request;
        }
    }
}
