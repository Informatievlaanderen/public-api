namespace Public.Api.Road.Extracts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ExtractController
    {
        [AllowAnonymous]
        [HttpGet("wegen/extract/overlappingtransactionzones.geojson")]
        public async Task<ActionResult> GetOverlappingTransactionZonesGeoJson(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                CreateBackendGetOverlappingTransactionZonesGeoJsonRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = new[] {"Retry-After"}
            };

            return response.ToActionResult(options);
        }

        private static RestRequest CreateBackendGetOverlappingTransactionZonesGeoJsonRequest() =>
            new RestRequest("extracts/overlappingtransactionzones.geojson");
    }
}
