namespace Public.Api.Road.Uploads
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public partial class UploadController
    {
        [HttpPost("wegen/upload/fc")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Post(
            IFormFile archive,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendUploadBeforeFeatureCompareRequest(archive),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendUploadBeforeFeatureCompareRequest(IFormFile archive) =>
            new HttpRequestMessage(HttpMethod.Post, "upload/fc")
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
