namespace Public.Api.Road.Extracts
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/download/{downloadId}/uploads/feature-compare")]
        public async Task<ActionResult> PostFeatureCompareUpload(
            [FromRoute]string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            IFormFile archive,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage BackendRequest()
            {
                var request = CreateBackendHttpRequestMessage(HttpMethod.Post, $"extracts/download/{downloadId}/uploads/feature-compare");
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
