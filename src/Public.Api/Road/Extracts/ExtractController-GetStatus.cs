namespace Public.Api.Road.Extracts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road")]
        [HttpGet("wegen/extract/upload/{uploadId}/status")]
        public async Task<ActionResult> GetStatus(
            [FromRoute]string uploadId,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                () => CreateBackendUploadStatusRequest(uploadId),
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = new[] {"Retry-After"}
            };

            return response.ToActionResult(options);
        }

        private static RestRequest CreateBackendUploadStatusRequest(string uploadId) => new RestRequest("extracts/upload/{uploadId}/status")
            .AddParameter(nameof(uploadId), uploadId, ParameterType.UrlSegment);
    }
}
