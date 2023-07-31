namespace Public.Api.Road.Information
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Information;

    public partial class InformationController
    {
        [HttpPost("wegen/informatie/valideer-wkt")]
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        public async Task<ActionResult> PostValidateWktContourRequest(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromBody] ValidateWktContourRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                new RestRequest("information/validate-wkt", Method.Post)
                    .AddJsonBodyOrEmpty(body)
                    .AddHeaderAuthorization(actionContextAccessor);

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
