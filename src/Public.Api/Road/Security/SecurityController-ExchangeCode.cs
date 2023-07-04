namespace Public.Api.Road.Security
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;

    public partial class SecurityController
    {
        [HttpGet("wegen/security/exchange", Name = nameof(ExchangeCode))]
        public async Task<IActionResult> ExchangeCode(
            string code,
            string verifier,
            string? redirectUri,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest()
            {
                return new RestRequest("security/exchange")
                    .AddParameter("code", code)
                    .AddParameter("verifier", verifier)
                    .AddParameter("redirectUri", redirectUri)
                    ;
            }

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
