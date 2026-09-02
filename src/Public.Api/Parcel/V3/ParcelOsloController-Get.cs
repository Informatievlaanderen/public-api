namespace Public.Api.Parcel.V3
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
    using ParcelRegistry.Api.Oslo.Parcel.V3.Detail;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class ParcelOsloController
    {
        /// <summary>
        /// Vraag een perceel op (v3).
        /// </summary>
        /// <param name="objectId">Dit kan op 3 manieren:
        /// <ul>
        /// <li>objectidentificator van het perceel (CaPaKey waarbij forward slash `/` vervangen werd door koppelteken `-`): `11001B0001-00S000`</li>
        /// <li>de CaPaKey van het perceel: `11001B0001/00S000`</li>
        /// <li>de URL-gecodeerde CaPaKey van het perceel: `11001B0001%2F00S000`</li>
        /// </ul></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="osloV3ParcelToggle"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het perceel verwijderd is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{objectId}", Name = nameof(GetParcelV3))]
        [ApiOrder(ApiOrder.Parcel.V3 + 1)]
        [ProducesResponseType(typeof(ParcelDetailOsloV3Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(ParcelGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetParcelV3(
            [FromRoute] string objectId,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] OsloV3ParcelToggle osloV3ParcelToggle,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!osloV3ParcelToggle.FeatureEnabled)
                return NotFound();

            objectId = System.Net.WebUtility.UrlDecode(objectId).Replace("/", "-");
            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"oslo-v3/parcel:{objectId}";

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

        /// <summary>
        /// Vraag een perceel op via CaPaKey (v3).
        /// </summary>
        /// <param name="caPaKeyPart1">Het eerste deel van de CaPaKey van het perceel.</param>
        /// <param name="caPaKeyPart2">Het tweede deel van de CaPaKey van het perceel.</param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het perceel gevonden is.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als het perceel niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="410">Als het perceel verwijderd is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen/{caPaKeyPart1}/{caPaKeyPart2}", Name = nameof(GetParcelV3CaPaKey))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(ParcelDetailOsloV3Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ParcelNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(ParcelGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetParcelV3CaPaKey(
            [FromRoute] string caPaKeyPart1,
            [FromRoute] string caPaKeyPart2,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var objectId = $"{caPaKeyPart1}-{caPaKeyPart2}";
            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"oslo-v3/parcel:{objectId}";

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

        private static RestRequest CreateBackendDetailRequest(string capaKey)
        {
            var request = new RestRequest("percelen/{capaKey}");
            request.AddParameter("capaKey", capaKey, ParameterType.UrlSegment);
            return request;
        }
    }
}
