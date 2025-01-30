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

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpGet("wegen/extract/download/{downloadId}/presignedurl")]
        public async Task<ActionResult> GetDownloadPreSignedUrlForExtract(
            [FromRoute] string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractGetDownloadToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var value = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "extracts/download/{downloadId}/presignedurl")
                    .AddParameter("downloadId", downloadId, ParameterType.UrlSegment);
        }
    }
}
