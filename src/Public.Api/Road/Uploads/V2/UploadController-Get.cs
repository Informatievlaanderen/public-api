namespace Public.Api.Road.Uploads.V2
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;

    public partial class UploadControllerV2
    {
        [HttpGet("wegen/upload/{identifier}", Name = nameof(RoadGetUploadV2))]
        public async Task<IActionResult> RoadGetUploadV2(
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
