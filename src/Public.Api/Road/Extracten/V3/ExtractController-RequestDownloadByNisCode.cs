namespace Public.Api.Road.Extracten.V3
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

    public partial class ExtractControllerV3
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extracten/downloadaanvragen/perniscode")]
        public async Task<ActionResult> PostDownloadRequestByNisCode(
            [FromBody] ExtractDownloadaanvraagPerNisCodeBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractDownloadRequestsByNisCodeV3Toggle toggle,
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
                CreateBackendRestRequest(Method.Post, "extracten/downloadaanvragen/perniscode")
                    .AddJsonBodyOrEmpty(body);
        }
    }
}
