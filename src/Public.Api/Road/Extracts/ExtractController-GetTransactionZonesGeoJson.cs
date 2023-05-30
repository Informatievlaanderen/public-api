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
        [HttpGet("wegen/extract/transactionzones.geojson")]
        public async Task<ActionResult> GetTransactionZonesGeoJson(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                CreateBackendGetTransactionZonesGeoJsonRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = new[] {"Retry-After"}
            };

            return response.ToActionResult(options);
        }

        private static RestRequest CreateBackendGetTransactionZonesGeoJsonRequest() =>
            new RestRequest("extracts/transactionzones.geojson");
    }
}
