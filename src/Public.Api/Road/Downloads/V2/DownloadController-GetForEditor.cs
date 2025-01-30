namespace Public.Api.Road.Downloads.V2
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public partial class DownloadControllerV2
    {
        [HttpGet("wegen/download/voor-editor")]
        public async Task<IActionResult> GetForEditorV2(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadDownloadGetForEditorToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
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
                CreateBackendHttpRequestMessage(HttpMethod.Get, "download/for-editor");
        }
    }
}
