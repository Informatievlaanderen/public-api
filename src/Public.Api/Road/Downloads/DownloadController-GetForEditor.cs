namespace Public.Api.Road.Downloads
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.FeatureToggles;

    public partial class DownloadController
    {
        [HttpGet("wegen/download/voor-editor")]
        public async Task<IActionResult> GetForEditor(
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
