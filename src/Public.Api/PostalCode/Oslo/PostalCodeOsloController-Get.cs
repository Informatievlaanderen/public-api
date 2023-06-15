namespace Public.Api.PostalCode.Oslo
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using PostalRegistry.Api.Oslo.PostalInformation.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class PostalCodeOsloController
    {
        /// <summary>
        /// Vraag postinfo voor een postcode op (v2).
        /// </summary>
        /// <param name="objectId">Identificator van de postinfo.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de postinfo voor een postcode gevonden is.</response>
        /// <response code="304">Als de postinfo voor een postcode niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als de postinfo voor een postcode niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo/{objectId}", Name = nameof(GetPostalCodeV2))]
        [ApiOrder(ApiOrder.PostalCode.V2 + 1)]
        [ProducesResponseType(typeof(PostalInformationOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PostalInformationOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(PostalInformationNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
        public async Task<IActionResult> GetPostalCodeV2(
            [FromRoute] string objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);

            var cacheKey = $"oslo/postalinfo:{objectId}";

            var value = await (CanGetFromCache(actionContextAccessor.ActionContext)
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

        private static RestRequest CreateBackendDetailRequest(string postalCode)
        {
            var request = new RestRequest("postcodes/{postalCode}");
            request.AddParameter("postalCode", postalCode, ParameterType.UrlSegment);
            return request;
        }
    }
}
