namespace Public.Api.Road.Downloads
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    public partial class DownloadController
    {
        [HttpGet("wegen/download/voor-editor")]
        public async Task<IActionResult> GetForEditor(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                CreateBackendDownloadForEditorRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendDownloadForEditorRequest() => new HttpRequestMessage(HttpMethod.Get, "download/for-editor");
    }
}
