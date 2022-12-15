namespace Public.Api.RoadSegments;

using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RestSharp;
using RoadRegistry.BackOffice.Abstractions.RoadSegments;
using RoadRegistry.BackOffice.Api.RoadSegments;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

public partial class RoadSegmentsController
{
    public const string UnlinkStreetNameRoute = "wegsegmenten/{id}/acties/straatnaamontkoppelen";

    /// <summary>
    ///     Ontkoppel een straatnaam van een wegsegment
    /// </summary>
    /// <param name="id">Identificator van het wegsegment.</param>
    /// <param name="request"></param>
    /// <param name="actionContextAccessor"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Als het wegsegment gevonden is.</response>
    /// <response code="404">Als het wegsegment niet gevonden kan worden.</response>
    /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPost(UnlinkStreetNameRoute, Name = nameof(PostUnlinkStreetName))]
    [ProducesResponseType(typeof(UnlinkStreetNameResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerRequestExample(typeof(PostUnlinkStreetNameParameters), typeof(PostUnlinkStreetNameParametersExamples))]
    //[SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De ETag van de response.")]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadSegmentNotFoundResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerOperation(Description = "Ontkoppel een linker- en/of rechterstraatnaam van een wegsegment waaraan momenteel een linker- en/of rechterstraatnaam gekoppeld is.")]
    public async Task<IActionResult> PostUnlinkStreetName(
        [FromRoute] string id,
        [FromBody] PostUnlinkStreetNameParameters request,
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        CancellationToken cancellationToken)
    {
        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

        RestRequest BackendRequest()
        {
            return CreateBackendRequestWithJsonBody(
                    UnlinkStreetNameRoute,
                    request,
                    Method.Post)
                .AddParameter(nameof(id), id, ParameterType.UrlSegment);
        }

        var value = await GetFromBackendWithBadRequestAsync(
            contentFormat.ContentType,
            BackendRequest,
            CreateDefaultHandleBadRequest(),
            problemDetailsHelper,
            cancellationToken: cancellationToken
        );

        return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
    }
}
