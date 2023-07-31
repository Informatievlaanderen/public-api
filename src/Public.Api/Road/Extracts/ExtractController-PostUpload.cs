namespace Public.Api.Road.Extracts
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/download/{downloadId}/uploads")]
        public async Task<ActionResult> PostUpload(
            [FromRoute]string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            IFormFile archive,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendUploadRequest(downloadId, archive),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendUploadRequest(
            string downloadId,
            IFormFile archive) => new HttpRequestMessage(HttpMethod.Post, $"extracts/download/{downloadId}/uploads")
        {
            Content = new StreamContent(archive.OpenReadStream()),
            Headers =
            {
                {HeaderNames.ContentDisposition, archive.ContentDisposition},
                {HeaderNames.ContentType, archive.ContentType}
            }
        };
    }
}
