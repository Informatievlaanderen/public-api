namespace Public.Api.Road.RoadSegments;

using System;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Common.Infrastructure.Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RestSharp;
using RoadRegistry.BackOffice.Api.RoadSegments;

public partial class RoadSegmentsController
{
    [HttpPost(EndPointRoot + "/wegsegmenten/{id}/acties/straatnaamkoppelen", Name = nameof(PostLinkStreetName))]
    public async Task<IActionResult> PostLinkStreetName(
        [FromRoute] string id,
        [FromBody] PostLinkStreetNameParameters request,
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
