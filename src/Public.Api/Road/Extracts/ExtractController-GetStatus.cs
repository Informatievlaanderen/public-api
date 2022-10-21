namespace Public.Api.Road.Extracts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ExtractController
    {
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

        private static IRestRequest CreateBackendUploadStatusRequest(string uploadId) => new RestRequest("extracts/upload/{uploadId}/status")
            .AddParameter(nameof(uploadId), uploadId, ParameterType.UrlSegment);
    }
}
