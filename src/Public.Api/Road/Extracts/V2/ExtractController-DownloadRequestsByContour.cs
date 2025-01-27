namespace Public.Api.Road.Extracts.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/downloadaanvragen/percontour")]
        public async Task<ActionResult> PostDownloadRequestByContourV2(
            [FromBody] DownloadExtractByContourRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractDownloadRequestsByContourToggle toggle,
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
                CreateBackendRestRequest(Method.Post, "extracts/downloadrequests/bycontour")
                    .AddJsonBodyOrEmpty(body);
        }
    }
}
