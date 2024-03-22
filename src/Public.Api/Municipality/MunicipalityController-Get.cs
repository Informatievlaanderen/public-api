// namespace Public.Api.Municipality
// {
//     using System.Threading;
//     using System.Threading.Tasks;
//     using Be.Vlaanderen.Basisregisters.Api.ETag;
//     using Be.Vlaanderen.Basisregisters.Api.Exceptions;
//     using Common.Infrastructure;
//     using Infrastructure;
//     using Infrastructure.Swagger;
//     using Marvin.Cache.Headers;
//     using Microsoft.AspNetCore.Http;
//     using Microsoft.AspNetCore.Mvc;
//     using Microsoft.AspNetCore.Mvc.Infrastructure;
//     using MunicipalityRegistry.Api.Legacy.Municipality.Responses;
//     using RestSharp;
//     using Swashbuckle.AspNetCore.Filters;
//     using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
//
//     public partial class MunicipalityController
//     {
//         /// <summary>
//         /// Vraag een gemeente op (v1).
//         /// </summary>
//         /// <param name="objectId">Identificator van de gemeente.</param>
//         /// <param name="actionContextAccessor"></param>
//         /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
//         /// <param name="cancellationToken"></param>
//         /// <response code="200">Als de gemeente gevonden is.</response>
//         /// <response code="304">Als de gemeente niet gewijzigd is ten opzicht van uw verzoek.</response>
//         /// <response code="400">Als uw verzoek foutieve data bevat.</response>
//         /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
//         /// <response code="404">Als de gemeente niet gevonden kan worden.</response>
//         /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
//         /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
//         /// <response code="500">Als er een interne fout is opgetreden.</response>
//         [HttpGet("gemeenten/{objectId}", Name = nameof(GetMunicipality))]
//         [ApiOrder(ApiOrder.Municipality.V1 + 1)]
//         [ProducesResponseType(typeof(MunicipalityResponse), StatusCodes.Status200OK)]
//         [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
//         [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
//         [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
//         [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
//         [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(MunicipalityNotFoundResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
//         [HttpCacheExpiration(MaxAge = DefaultDetailCaching)]
//         public async Task<IActionResult> GetMunicipality(
//             [FromRoute] int objectId,
//             [FromServices] IActionContextAccessor actionContextAccessor,
//             [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
//             CancellationToken cancellationToken = default)
//         {
//             var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
//
//             RestRequest BackendRequest() => CreateBackendDetailRequest(objectId);
//
//             var cacheKey = $"legacy/municipality:{objectId}";
//
//             var value = await (CanGetFromCache(actionContextAccessor.ActionContext)
//                 ? GetFromCacheThenFromBackendAsync(
//                     contentFormat.ContentType,
//                     BackendRequest,
//                     cacheKey,
//                     CreateDefaultHandleBadRequest(),
//                     cancellationToken)
//                 : GetFromBackendAsync(
//                     contentFormat.ContentType,
//                     BackendRequest,
//                     CreateDefaultHandleBadRequest(),
//                     cancellationToken));
//
//             return new BackendResponseResult(value);
//         }
//
//         private static RestRequest CreateBackendDetailRequest(int nisCode)
//         {
//             var request = new RestRequest("gemeenten/{nisCode}");
//             request.AddParameter("nisCode", nisCode, ParameterType.UrlSegment);
//             return request;
//         }
//     }
// }
