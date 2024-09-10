namespace Public.Api.Road.Extracts.V2
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractControllerV2
    {
        [HttpPost("wegen/extract/overlapping/perniscode")]
        public async Task<ActionResult> ListOverlappingByNisCode(
            [FromBody] ExtractsController.ListOverlappingByNisCodeParameters body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "extracts/overlapping/byniscode")
                    .AddJsonBody(body);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);
        }
    }
}
