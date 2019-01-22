namespace Public.Api.Parcel
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json.Converters;
    using ParcelRegistry.Api.Legacy.Parcel.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;

    public partial class ParcelController
    {
        /// <summary>
        /// Vraag een perceel op.
        /// </summary>
        /// <param name="perceelId">Identificator van het perceel.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="304">Als het perceel niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{perceelId}")]
        [ProducesResponseType(typeof(ParcelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 30 * 24 * 60 * 60)] // Days, Hours, Minutes, Second
        public async Task<IActionResult> Get(
            [FromRoute] string perceelId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await Get(
                null,
                perceelId,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een perceel op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="perceelId">Identificator van het perceel.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="304">Als het perceel niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{perceelId}.{format}")]
        [ProducesResponseType(typeof(ParcelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 30 * 24 * 60 * 60)] // Days, Hours, Minutes, Second
        public async Task<IActionResult> Get(
            [FromRoute] string format,
            [FromRoute] string perceelId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            const string notFoundExceptionMessage = "Onbestaand perceel.";

            void HandleNotFound(HttpStatusCode statusCode)
            {
                switch (statusCode)
                {
                    case HttpStatusCode.NotAcceptable:
                        throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);

                    case HttpStatusCode.BadRequest:
                        throw new ApiException("Ongeldige vraag.", StatusCodes.Status400BadRequest);

                    case HttpStatusCode.NotFound:
                        throw new ApiException(string.IsNullOrWhiteSpace(notFoundExceptionMessage)
                            ? "Niet gevonden."
                            : notFoundExceptionMessage, StatusCodes.Status404NotFound);
                }
            }

            RestRequest BackendRequest() => CreateBackendDetailRequest(perceelId);

            var cacheKey = $"legacy/parcel:{perceelId}";

            var value = await (CacheToggle.FeatureEnabled
                ? GetFromCacheThenFromBackendAsync(format, BackendRequest, cacheKey, Request.GetTypedHeaders(), HandleNotFound, cancellationToken)
                : GetFromBackendAsync(format, BackendRequest, Request.GetTypedHeaders(), HandleNotFound, cancellationToken));

            return new BackendResponseResult(value);
        }

        protected RestRequest CreateBackendDetailRequest(string perceelId)
        {
            var request = new RestRequest("percelen/{perceelId}");
            request.AddParameter("perceelId", perceelId, ParameterType.UrlSegment);
            return request;
        }
    }
}
