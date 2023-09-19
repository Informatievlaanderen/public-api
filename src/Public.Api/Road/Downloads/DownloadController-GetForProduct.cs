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
        [HttpGet("wegen/download/voor-product/{datum}")]
        public async Task<IActionResult> GetForProduct(
            [FromRoute] string datum,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendHttpRequestMessage(HttpMethod.Get, $"download/for-product/{datum}"),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }
    }
}
