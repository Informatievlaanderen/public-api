namespace Public.Api.Road.Security
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public partial class SecurityController
    {
        [HttpGet("wegen/security/exchange", Name = nameof(ExchangeCode))]
        public async Task<IActionResult> ExchangeCode(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendExchangeCodeRequest(),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendExchangeCodeRequest() =>
            new HttpRequestMessage(HttpMethod.Get, "security/exchange");

    }
}
