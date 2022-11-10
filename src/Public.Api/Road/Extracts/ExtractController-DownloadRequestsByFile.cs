namespace Public.Api.Road.Extracts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractController
    {
        [HttpPost("wegen/extract/downloadaanvragen/perbestand")]
        public async Task<ActionResult> PostDownloadRequestByFile(
            [FromBody] DownloadExtractByFileRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() => CreateBackendDownloadRequestByFile(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);
        }

        private static RestRequest CreateBackendDownloadRequestByFile(DownloadExtractByFileRequestBody body) =>
            new RestRequest("extracts/downloadrequests/byfile", Method.Post)
            .AddJsonBodyOrEmpty(body);
    }
}
