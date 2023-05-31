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
        [ApiKeyAuth("Road")]
        [HttpPost("wegen/extract/download/{downloadId}/uploads/feature-compare")]
        public async Task<ActionResult> PostFeatureCompareUpload(
            [FromRoute]string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            IFormFile archive,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendFeatureCompareUploadRequest(downloadId, archive),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendFeatureCompareUploadRequest(
            string downloadId,
            IFormFile archive) => new HttpRequestMessage(HttpMethod.Post, $"extracts/download/{downloadId}/uploads/feature-compare")
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
