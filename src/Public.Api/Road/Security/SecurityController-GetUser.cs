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
        [HttpGet("wegen/security/user", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                _httpClient,
                () => CreateBackendGetUserRequest(),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken
            );

            return response.ToActionResult();
        }

        private static HttpRequestMessage CreateBackendGetUserRequest() =>
            new HttpRequestMessage(HttpMethod.Get, "security/user");

    }
}
