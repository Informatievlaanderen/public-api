namespace Public.Api.Building.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.BackOffice.Abstractions.Building.Requests;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingBackOfficeController
    {
        public const string PlanBuildingRoute = "gebouwen/acties/plannen";

        /// <summary>
        /// Plan een gebouw in.
        /// </summary>
        /// <param name="planBuildingRequest"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="planBuildingToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als u niet geauthenticeerd bent om deze actie uit te voeren.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ApiOrder(ApiOrder.Building.Edit + 1)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(PlanBuildingRequest), typeof(PlanBuildingRequestExamples))]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerOperation(Description = "Voer een nieuw geschetst gebouw in met status `gepland`.")]
        [HttpPost(PlanBuildingRoute, Name = nameof(PlanBuilding))]
        public async Task<IActionResult> PlanBuilding(
                    [FromBody] PlanBuildingRequest planBuildingRequest,
                    [FromServices] IActionContextAccessor actionContextAccessor,
                    [FromServices] ProblemDetailsHelper problemDetailsHelper,
                    [FromServices] PlanBuildingToggle planBuildingToggle,
                    CancellationToken cancellationToken = default)
        {
            if (!planBuildingToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() =>
                CreateBackendRequestWithJsonBody(PlanBuildingRoute, planBuildingRequest, Method.Post).AddHeaderAuthorization(actionContextAccessor);

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
