namespace Public.Api.Road.Uploads.V2
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public partial class UploadControllerV2
    {
        [HttpGet("wegen/upload/{identifier}", Name = nameof(RoadGetUploadV2))]
        public async Task<IActionResult> RoadGetUploadV2(
            [FromRoute] string identifier,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadJobsToggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();

            HttpRequestMessage BackendRequest() =>
                CreateBackendHttpRequestMessage(HttpMethod.Get, $"upload/{identifier}");
        }
    }
}
