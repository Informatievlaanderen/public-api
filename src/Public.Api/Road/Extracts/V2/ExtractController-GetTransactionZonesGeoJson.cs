namespace Public.Api.Road.Extracts.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ExtractControllerV2
    {
        [HttpGet("wegen/extract/transactionzones.geojson")]
        public async Task<ActionResult> GetTransactionZonesGeoJsonV2(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractGetTransactionZonesGeoJsonToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = ["Retry-After"]
            };

            return response.ToActionResult(options);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "extracts/transactionzones.geojson");
        }
    }
}
