namespace Public.Api.Road.Extracts.V2
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Infrastructure.Controllers.Attributes;
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/overlapping/percontour")]
        public async Task<ActionResult> ListOverlappingByContour(
            [FromBody] ExtractsController.ListOverlappingByContourParameters body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "extracts/overlapping/bycontour")
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
