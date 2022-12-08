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
using Swashbuckle.AspNetCore.Filters;

public partial class RoadSegmentsController
{
    public const string LinkStreetNameRoute = "wegsegmenten/{id}/acties/straatnaamkoppelen";

    [HttpPost(LinkStreetNameRoute, Name = nameof(PostLinkStreetName))]
    [ProducesResponseType(typeof(LinkStreetNameResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LinkStreetNameResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> PostLinkStreetName(
        [FromRoute] string id,
        [FromBody] PostLinkStreetNameParameters request,
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        CancellationToken cancellationToken)
    {
        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

        RestRequest BackendRequest()
        {
            return CreateBackendRequestWithJsonBody(
                    LinkStreetNameRoute,
                    request,
                    Method.Post)
                .AddParameter(nameof(id), id);
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
