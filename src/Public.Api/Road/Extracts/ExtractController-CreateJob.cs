namespace Public.Api.Road.Extracts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using RoadRegistry.BackOffice.Abstractions.Jobs;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class ExtractController
    {
        [ProducesResponseType(typeof(GetPresignedUploadUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("wegen/extract/download/{downloadId}/jobs", Name = nameof(RoadExtractCreateJob))]
        public async Task<IActionResult> RoadExtractCreateJob(
            [FromRoute] string downloadId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractCreateJobToggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => new RestRequest($"extracts/download/{downloadId}/jobs", Method.Post)
                .AddHeaderAuthorization(actionContextAccessor);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
        }
    }
}
