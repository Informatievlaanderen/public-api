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

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extracten/{downloadId}/upload")]
        public async Task<ActionResult> UploadExtract(
            [FromRoute] string downloadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadUploadExtractToggle toggle,
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
                CreateBackendRestRequest(Method.Post, "extracten/{downloadId}/upload")
                    .AddParameter("downloadId", downloadId, ParameterType.UrlSegment);
        }
    }
}
