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
        [HttpGet("wegen/extract/municipalities.geojson")]
        public async Task<ActionResult> GetMunicipalitiesGeoJson(
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                CreateBackendGetMunicipalitiesGeoJsonRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            var options = new BackendResponseResultOptions
            {
                ForwardHeaders = new[] {"Retry-After"}
            };

            return response.ToActionResult(options);
        }

        private static RestRequest CreateBackendGetMunicipalitiesGeoJsonRequest() =>
            new RestRequest("extracts/municipalities.geojson");
    }
}
