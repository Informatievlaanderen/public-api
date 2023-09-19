namespace Public.Api.Road.Downloads
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class DownloadController
    {
        [HttpGet("wegen/download/voor-editor")]
        public async Task<IActionResult> GetForEditor(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendHttpRequestMessage(HttpMethod.Get, "download/for-editor"),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }
    }
}
