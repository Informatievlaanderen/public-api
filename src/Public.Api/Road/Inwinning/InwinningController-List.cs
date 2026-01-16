namespace Public.Api.Road.Inwinning
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Inwinning;

    public partial class InwinningControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpGet("wegen/inwinning/extracten")]
        public async Task<ActionResult> ListInwinningExtracten(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadInwinningListToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "inwinning/extracten");
        }
    }
}
