namespace Public.Api.Road.Security
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class SecurityController
    {
        [HttpGet("wegen/security/exchange", Name = nameof(ExchangeCode))]
        public async Task<IActionResult> ExchangeCode(
            string code,
            string verifier,
            string? redirectUri,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "security/exchange")
                    .AddParameter("code", code)
                    .AddParameter("verifier", verifier)
                    .AddParameter("redirectUri", redirectUri);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken
            );

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }
    }
}
