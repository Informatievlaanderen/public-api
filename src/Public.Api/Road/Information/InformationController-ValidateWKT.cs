namespace Public.Api.Road.Information
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Extracts;
    using RoadRegistry.BackOffice.Api.Information;

    public partial class InformationController
    {
        [HttpPost("wegen/informatie/valideer-wkt")]
        public async Task<ActionResult> PostValidateWktContourRequest(
            [FromBody] ValidateWktContourRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() => CreateBackendValidateWktContourRequest(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);
        }

        private static RestRequest CreateBackendValidateWktContourRequest(ValidateWktContourRequestBody body) =>
            new RestRequest("information/validate-wkt", Method.Post)
                .AddJsonBodyOrEmpty(body);
    }
}
