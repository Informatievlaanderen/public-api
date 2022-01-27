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
        [HttpPost("wegen/extract/downloadaanvragen/perniscode")]
        public async Task<ActionResult> PostDownloadRequestByNisCode(
            [FromBody] DownloadExtractByNisCodeRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendDownloadRequestRequestByNisCode(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken);

            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendDownloadRequestRequestByNisCode(DownloadExtractByNisCodeRequestBody body) => new RestRequest("extracts/downloadrequests/bycontour")
            .AddParameter(nameof(body), body, ParameterType.RequestBody);

        // TODO: use contract in nuget package
        public class DownloadExtractByNisCodeRequestBody
        {
            public string NisCode { get; set; }
            public int Buffer { get; set; }
        }
    }
}
