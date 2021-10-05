namespace Public.Api.Road.Uploads
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public partial class UploadController
    {
        [HttpGet("wegen/upload/{identifier}", Name = nameof(GetUpload))]
        public async Task<IActionResult> GetUpload(
            [FromRoute] string identifier,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendGetUploadRequest(identifier),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendGetUploadRequest(string identifier) =>
            new HttpRequestMessage(HttpMethod.Get, $"upload/{identifier}");

    }
}
