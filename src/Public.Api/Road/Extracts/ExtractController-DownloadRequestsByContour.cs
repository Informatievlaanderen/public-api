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
        [HttpPost("wegen/extract/downloadaanvragen/percontour")]
        public async Task<ActionResult> PostDownloadRequestByContour(
            [FromBody] DownloadExtractByContourRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendDownloadRequestRequestByContour(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken);

            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendDownloadRequestRequestByContour(DownloadExtractByContourRequestBody body) => new RestRequest("extracts/downloadrequests/bycontour")
            .AddParameter(nameof(body), body, ParameterType.RequestBody);

        // TODO: use contract in nuget package
        public class DownloadExtractByContourRequestBody
        {
            public string Contour { get; set; }
            public int Buffer { get; set; }
        }
    }
}
