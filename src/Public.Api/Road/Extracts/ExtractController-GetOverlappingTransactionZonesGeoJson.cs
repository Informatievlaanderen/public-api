namespace Public.Api.Road.Extracts
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class ExtractController
    {
        [HttpGet("wegen/extract/overlappingtransactionzones.geojson")]
        public async Task<ActionResult> GetOverlappingTransactionZonesGeoJson(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "extracts/overlappingtransactionzones.geojson");

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
        }
    }
}
