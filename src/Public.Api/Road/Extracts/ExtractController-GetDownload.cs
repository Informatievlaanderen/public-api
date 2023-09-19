namespace Public.Api.Road.Extracts
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/download/{downloadId}")]
        public async Task<ActionResult> PostDownloadRequest(
            [FromRoute]string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendHttpRequestMessage(HttpMethod.Get, $"extracts/download/{downloadId}"),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }
    }
}
