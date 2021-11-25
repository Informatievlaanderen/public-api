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
        [HttpPost("wegen/extract/downloadaanvragen")]
        public async Task<ActionResult> PostDownloadRequest(
            [FromBody]DownloadExtractRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendDownloadRequestRequest(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken);

            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendDownloadRequestRequest(DownloadExtractRequestBody body) => new RestRequest("extracts/downloadrequests")
            .AddParameter(nameof(body), body, ParameterType.RequestBody);
    }
}
