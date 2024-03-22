// namespace Public.Api.Municipality
// {
//     using System.Threading;
//     using System.Threading.Tasks;
//     using Be.Vlaanderen.Basisregisters.Api.Exceptions;
//     using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
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
//         /// Vraag het totaal aantal gemeenten op.
//         /// </summary>
//         /// <param name="actionContextAccessor"></param>
//         /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
//         /// <param name="cancellationToken"></param>
//         /// <response code="200">Als de opvraging van het totaal aantal gemeenten gelukt is.</response>
//         /// <response code="400">Als uw verzoek foutieve data bevat.</response>
//         /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
//         /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
//         /// <response code="500">Als er een interne fout is opgetreden.</response>
//         [HttpGet("gemeenten/totaal-aantal", Name = nameof(CountMunicipalities))]
//         [ApiOrder(ApiOrder.Municipality.V1 + 3)]
//         [ApiExplorerSettings(IgnoreApi = true)]
//         [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
//         [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
//         [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
//         [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
//         [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
//         [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
//         [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
//         [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
//         [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
//         [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
//         public async Task<IActionResult> CountMunicipalities(
//             [FromServices] IActionContextAccessor actionContextAccessor,
//             [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
//             CancellationToken cancellationToken = default)
//         {
//             var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
//
//             RestRequest BackendRequest() => CreateBackendCountRequest();
//
//             return new BackendResponseResult(
//                 await GetFromBackendAsync(
//                     contentFormat.ContentType,
//                     BackendRequest,
//                     CreateDefaultHandleBadRequest(),
//                     cancellationToken));
//         }
//
//         private static RestRequest CreateBackendCountRequest() => new RestRequest("gemeenten/totaal-aantal");
//     }
// }
