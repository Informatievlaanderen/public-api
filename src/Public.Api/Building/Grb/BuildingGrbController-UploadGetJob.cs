namespace Public.Api.Building.Grb
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingGrbController
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("gebouwen/uploads/jobs/{jobId}", Name = nameof(BuildingGrbUploadGetJob))]
        public async Task<IActionResult> BuildingGrbUploadGetJob(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] BuildingGrbUploadJobToggle buildingGrbUploadJobToggle,
            [FromRoute] Guid jobId,
            CancellationToken cancellationToken = default)
        {
            if (!buildingGrbUploadJobToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => new RestRequest("uploads/jobs/{jobId}", Method.Get)
                .AddParameter("jobId", jobId, ParameterType.UrlSegment)
                .AddHeaderAuthorization(actionContextAccessor);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
        }
    }
}
