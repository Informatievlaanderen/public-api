namespace Public.Api.Building
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.Legacy.Building.Responses;
    using Common.Infrastructure;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingController
    {
        /// <summary>
        /// Vraag de referenties van een gebouw op.
        /// </summary>
        /// <param name="objectId">Identificator van het gebouw.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">De referenties van het gebouw.</response>
        /// <response code="304">Als de gebouw referenties niet gewijzigd zijn ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als het gebouw niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het gebouw verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen/{objectId}/referenties")]
        [ProducesResponseType(typeof(BuildingReferencesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingReferencesResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BuildingNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(BuildingGoneResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetBuildingReferences(
            [FromRoute] int objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await GetBuildingReferencesWithFormat(
                null,
                objectId,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag de referenties van een gebouw op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="objectId">Identificator van het gebouw.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">De referenties van het gebouw.</response>
        /// <response code="304">Als de gebouw referenties niet gewijzigd zijn ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als het gebouw niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het gebouw verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen/{objectId}.{format}/referenties")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(BuildingReferencesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingReferencesResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BuildingNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(BuildingGoneResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetBuildingReferencesWithFormat(
            [FromRoute] string format,
            [FromRoute] int objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = DetermineAndSetContentFormat(format, actionContextAccessor, Request);

            RestRequest BackendRequest() => CreateBackendReferencesRequest(objectId);

            var value = await GetFromBackendAsync(format, BackendRequest, Request.GetTypedHeaders(), CreateDefaultHandleBadRequest(), cancellationToken);

            return new BackendResponseResult(value);
        }

        private static RestRequest CreateBackendReferencesRequest(int buildingId)
        {
            var request = new RestRequest("gebouwen/{buildingId}/referenties");
            request.AddParameter("buildingId", buildingId, ParameterType.UrlSegment);
            return request;
        }
    }
}
