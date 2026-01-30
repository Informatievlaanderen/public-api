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
        [HttpPost("wegen/inwinning/downloadaanvraag")]
        public async Task<ActionResult> RequestInwinningExtract(
            [FromBody] InwinningExtractDownloadaanvraagBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadInwinningRequestExtractToggle toggle,
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

            return new BackendResponseResult(response, BackendResponseResultOptions.ForBackOffice());

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "inwinning/downloadaanvraag")
                    .AddJsonBodyOrEmpty(body);
        }
    }
}
