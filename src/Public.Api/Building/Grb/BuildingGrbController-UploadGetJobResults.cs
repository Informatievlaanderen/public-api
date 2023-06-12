namespace Public.Api.Building.Grb
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;

    public partial class BuildingGrbController
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("gebouwen/uploads/jobs/{objectId}/results", Name = nameof(BuildingGrbUploadJobsResults))]
        public async Task<IActionResult> BuildingGrbUploadJobsResults(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] BuildingGrbUploadJobToggle buildingGrbUploadJobToggle,
            [FromRoute] Guid objectId,
            CancellationToken cancellationToken = default)
        {
            if (!buildingGrbUploadJobToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => new RestRequest("uploads/jobs/{objectId}/results", Method.Get)
                .AddParameter("objectId", objectId, ParameterType.UrlSegment)
                .AddHeaderAuthorization(actionContextAccessor);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            if ((int)value.StatusCode >= 200 && (int)value.StatusCode <= 299)
            {
                var preSignedUrlResponse =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadJobResultsPreSignedUrlResponse>(value.Content);

                return new RedirectResult(preSignedUrlResponse.GetUrl, false);
            }

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
        }
    }

    public sealed record DownloadJobResultsPreSignedUrlResponse(Guid JobId, string GetUrl);
}
