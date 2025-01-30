namespace Public.Api.Road.Extracts
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.FeatureToggles;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpGet("wegen/extract/upload/{uploadId}/status")]
        public async Task<ActionResult> GetStatus(
            [FromRoute]string uploadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractGetStatusToggle toggle,
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

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = new[] {"Retry-After"}
            };

            return response.ToActionResult(options);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "extracts/upload/{uploadId}/status")
                    .AddParameter(nameof(uploadId), uploadId, ParameterType.UrlSegment);
        }
    }
}
