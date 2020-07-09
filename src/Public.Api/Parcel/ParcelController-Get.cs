namespace Public.Api.Parcel
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Newtonsoft.Json.Converters;
    using ParcelRegistry.Api.Legacy.Parcel.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class ParcelController
    {
        /// <summary>
        /// Vraag een perceel op.
        /// </summary>
        /// <param name="objectId">Objectidentificator van het perceel (CaPaKey waarbij forward slash `/` vervangen werd door koppelteken `-`).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="304">Als het perceel niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het perceel verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{objectId}")]
        [ProducesResponseType(typeof(ParcelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(ParcelGoneResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetParcel(
            [FromRoute] string objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await GetParcelWithFormat(
                null,
                objectId,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een perceel op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="objectId">Objectidentificator van het perceel (CaPaKey waarbij forward slash `/` vervangen werd door koppelteken `-`).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="304">Als het perceel niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het perceel verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{objectId}.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(ParcelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(ParcelGoneResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetParcelWithFormat(
            [FromRoute] string format,
            [FromRoute] string objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = ContentFormat.For(format, actionContextAccessor, Request);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"legacy/parcel:{objectId}";

            var value = await (CacheToggle.FeatureEnabled
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

            return new BackendResponseResult(value);
        }

        private static RestRequest CreateBackendDetailRequest(string capaKey)
        {
            var request = new RestRequest("percelen/{capaKey}");
            request.AddParameter("capaKey", capaKey, ParameterType.UrlSegment);
            return request;
        }
    }
}
