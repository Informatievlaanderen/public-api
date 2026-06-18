namespace Public.Api.Building.Grb
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingGrbController
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpDelete("gebouwen/uploads/jobs/{objectId}", Name = nameof(BuildingGrbUploadCancelJob))]
        public async Task<IActionResult> BuildingGrbUploadCancelJob(
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] BuildingGrbUploadJobToggle buildingGrbUploadJobToggle,
            [FromRoute] Guid objectId,
            CancellationToken cancellationToken = default)
        {
            if (!buildingGrbUploadJobToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);

            RestRequest BackendRequest() => new RestRequest("uploads/jobs/{objectId}", Method.Delete)
                .AddParameter("objectId", objectId, ParameterType.UrlSegment)
                .AddHeaderAuthorization(httpContextAccessor);

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
