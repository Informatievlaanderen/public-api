namespace Public.Api.Road.Extracten
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpGet("wegen/extracten")]
        public async Task<ActionResult> List(
            [FromQuery] bool? eigenExtracten,
            [FromQuery] int? page,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadListExtractsToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var value = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "extracten")
                    .AddParameter(nameof(eigenExtracten), eigenExtracten, ParameterType.QueryString)
                    .AddParameter(nameof(page), page, ParameterType.QueryString)
                ;
        }
    }
}
