namespace Public.Api.Road.Uploads
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class UploadController
    {
        [HttpGet("wegen/upload/{identifier}", Name = nameof(GetUpload))]
        public async Task<IActionResult> GetUpload(
            [FromRoute] string identifier,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage BackendRequest() =>
                CreateBackendHttpRequestMessage(HttpMethod.Get, $"upload/{identifier}");

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
