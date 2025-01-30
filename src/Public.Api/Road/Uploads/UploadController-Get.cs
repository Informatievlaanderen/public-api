namespace Public.Api.Road.Uploads
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.FeatureToggles;

    public partial class UploadController
    {
        [HttpGet("wegen/upload/{identifier}", Name = nameof(RoadGetUpload))]
        public async Task<IActionResult> RoadGetUpload(
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
