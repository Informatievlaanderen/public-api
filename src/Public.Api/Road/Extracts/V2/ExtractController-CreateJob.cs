namespace Public.Api.Road.Extracts.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Abstractions.Jobs;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class ExtractControllerV2
    {
        [ProducesResponseType(typeof(GetPresignedUploadUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("wegen/extract/download/{downloadId}/jobs", Name = nameof(RoadExtractCreateJobV2))]
        public async Task<IActionResult> RoadExtractCreateJobV2(
            [FromRoute] string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadJobsToggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var value = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "extracts/download/{downloadId}/jobs")
                    .AddParameter("downloadId", downloadId, ParameterType.UrlSegment);
        }
    }
}
