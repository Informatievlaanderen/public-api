namespace Public.Api.Road.Information
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Information;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class InformationController
    {
        [HttpPost("wegen/informatie/valideer-wkt")]
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        public async Task<ActionResult> PostValidateWktContourRequest(
            [FromBody] ValidateWktContourRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, "information/validate-wkt")
                    .AddJsonBodyOrEmpty(body);

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
