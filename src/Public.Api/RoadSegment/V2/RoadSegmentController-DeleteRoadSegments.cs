namespace Public.Api.RoadSegment.V2
{
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.RoadSegments;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.FeatureToggles;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentControllerV2
    {
        private const string DeleteRoadSegmentsRoute = "wegsegmenten/acties/verwijderen";

        /// <summary>
        ///     Verwijder wegsegmenten (v2).
        /// </summary>
        /// <param name="request">Identificators van de wegsegmenten.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als wegsegmenten gevonden zijn.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(DeleteRoadSegmentsRoute, Name = nameof(DeleteRoadSegmentsV2))]
        [ApiOrder(ApiOrder.Road.RoadSegment.DeleteRoadSegments)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(DeleteRoadSegmentsV2),
            Description = "Verwijder wegsegmenten.",
            Authorize = Scopes.DvWrUitzonderingenBeheer
        )]
        public async Task<IActionResult> DeleteRoadSegmentsV2(
            [FromBody] DeleteRoadSegmentsParameters request,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] DeleteRoadSegmentsToggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response, BackendResponseResultOptions.ForBackOffice());

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, DeleteRoadSegmentsRoute)
                    .AddJsonBodyOrEmpty(request);
        }
    }
}
