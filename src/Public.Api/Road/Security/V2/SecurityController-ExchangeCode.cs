namespace Public.Api.Road.Security.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;

    public partial class SecurityControllerV2
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
