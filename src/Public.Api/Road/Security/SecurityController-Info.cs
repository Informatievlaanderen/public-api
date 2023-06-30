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
        [HttpGet("wegen/security/info", Name = nameof(Info))]
        public async Task<IActionResult> Info(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendInfoRequest(),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendInfoRequest() =>
            new HttpRequestMessage(HttpMethod.Get, "security/info");

    }
}
