namespace Public.Api.Road.Extracten
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
    using RoadRegistry.BackOffice.Api.Extracten;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extracten/overlapping/perniscode")]
        public async Task<ActionResult> ListOverlappingByNisCode(
            [FromBody] ExtractenController.GetOverlappingPerNisCodeBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadOverlappingExtractsByNisCodeToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "extracten/overlapping/perniscode")
                    .AddJsonBody(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);
        }
    }
}
