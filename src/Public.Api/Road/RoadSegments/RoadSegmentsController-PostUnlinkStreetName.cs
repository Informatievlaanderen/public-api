namespace Public.Api.Road.RoadSegments;

using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Common.Infrastructure.Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RestSharp;
using RoadRegistry.BackOffice.Api.RoadSegments;

public partial class RoadSegmentsController
{
    [HttpPost(EndPointRoot + "/wegsegmenten/{id}/acties/straatnaamontkoppelen", Name = nameof(PostUnlinkStreetName))]
    public async Task<IActionResult> PostUnlinkStreetName(
        [FromRoute] string id,
        [FromBody] PostUnlinkStreetNameParameters request,
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        CancellationToken cancellationToken)
    {
        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

        RestRequest BackendRequest() => CreateBackendRequestWithJsonBody(
            Request.GetPathAfterSection(EndPointRoot),
            request,
            Method.Post);

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
