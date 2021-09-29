namespace Public.Api.Road.Downloads
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

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
                () => CreateBackendProductRequest(datum),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendProductRequest(string datum) => new HttpRequestMessage(HttpMethod.Get, $"download/for-product/{datum}");
    }
}
