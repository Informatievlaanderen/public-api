namespace Public.Api.Road.Uploads
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class UploadController
    {
        [HttpPost("wegen/upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> PostUploadAfterFeatureCompare(
            IFormFile archive,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage BackendRequest()
            {
                var request = CreateBackendHttpRequestMessage(HttpMethod.Post, "upload");
                request.Content = new StreamContent(archive.OpenReadStream());
                request.Headers.Add(HeaderNames.ContentDisposition, archive.ContentDisposition);
                request.Headers.Add(HeaderNames.ContentType, archive.ContentType);
                return request;
            }

            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }
    }
}
