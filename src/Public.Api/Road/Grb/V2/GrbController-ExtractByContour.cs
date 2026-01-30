namespace Public.Api.Road.Grb.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Abstractions.Extracts;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class GrbControllerV2
    {
        [ProducesResponseType(typeof(DownloadExtractResponseBody), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("wegen/grb/extract/percontour", Name = nameof(ExtractByContour))]
        public async Task<IActionResult> ExtractByContour(
            [FromBody] DownloadExtractRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadGrbExtractByContourToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response, BackendResponseResultOptions.ForBackOffice());

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "grb/extracts/bycontour")
                    .AddJsonBodyOrEmpty(body);
        }
    }
}
