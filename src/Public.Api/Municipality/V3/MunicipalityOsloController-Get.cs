namespace Public.Api.Municipality.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using MunicipalityRegistry.Api.Oslo.Municipality.V3.Responses;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class MunicipalityOsloController
    {
        /// <summary>
        /// Vraag een gemeente op (v3).
        /// </summary>
        /// <param name="objectId">Identificator van de gemeente.</param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="osloV3MunicipalityToggle"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de gemeente gevonden is.</response>
        /// <response code="304">Als de gemeente niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als de gemeente niet gevonden kan worden.</response>
        /// <response code="410">Als de gemeente verwijderd is.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [ApiOrder(ApiOrder.Municipality.V3 + 1)]
        [HttpGet("gemeenten/{objectId}", Name = nameof(GetMunicipalityV3))]
        [ProducesResponseType(typeof(MunicipalityOsloV3Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(MunicipalityNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(MunicipalityGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetMunicipalityV3(
            [FromRoute] int objectId,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] OsloV3MunicipalityToggle osloV3MunicipalityToggle,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!osloV3MunicipalityToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"oslo-v3/municipality:{objectId}";

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

            return new BackendResponseResult(value);
        }

        private static RestRequest CreateBackendDetailRequest(int nisCode)
        {
            var request = new RestRequest("gemeenten/{nisCode}");
            request.AddParameter("nisCode", nisCode, ParameterType.UrlSegment);
            return request;
        }
    }
}
