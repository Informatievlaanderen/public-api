namespace Public.Api.Road.Security
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class SecurityController
    {
        [HttpGet("wegen/security/user", Name = nameof(GetUser))]
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        public async Task<IActionResult> GetUser(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "security/user");

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
