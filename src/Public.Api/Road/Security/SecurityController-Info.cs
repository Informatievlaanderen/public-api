namespace Public.Api.Road.Security
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;

    public partial class SecurityController
    {
        [HttpGet("wegen/security/info", Name = nameof(Info))]
        public async Task<IActionResult> Info(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest()
            {
                return new RestRequest("security/info");
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
