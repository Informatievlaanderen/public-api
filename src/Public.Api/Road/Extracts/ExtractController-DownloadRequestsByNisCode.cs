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
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractController
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/downloadaanvragen/perniscode")]
        public async Task<ActionResult> PostDownloadRequestByNisCode(
            [FromBody] DownloadExtractByNisCodeRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() => CreateBackendDownloadRequestByNisCode(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);
        }

        private static RestRequest CreateBackendDownloadRequestByNisCode(DownloadExtractByNisCodeRequestBody body) =>
            new RestRequest("extracts/downloadrequests/byniscode", Method.Post)
            .AddJsonBodyOrEmpty(body);
    }
}
