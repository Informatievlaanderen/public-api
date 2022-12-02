namespace Public.Api.Road.RoadSegments;

using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Common.Infrastructure.Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RestSharp;
using Swashbuckle.AspNetCore.Filters;

public partial class RoadSegmentsController
{
    [HttpPost(RootEndPoint + "/wegsegmenten/{id}/acties/straatnaamontkoppelen", Name = nameof(PostUnlinkStreetName))]
    //[ProducesResponseType(typeof(StreetNameListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
    //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(PostLinkStreetNameResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> PostUnlinkStreetName(
        [FromRoute] string id,
        [FromBody] RoadRegistry.BackOffice.Api.RoadSegments.RoadSegmentsController.PostUnlinkStreetNameParameters request,
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        CancellationToken cancellationToken)
    {
        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

        RestRequest BackendRequest()
        {
            return CreateBackendRequestWithJsonBody(
                Request.GetPathAfterRoutePart(RootEndPoint),
                request,
                Method.Post);
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
