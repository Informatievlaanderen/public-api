namespace Public.Api.RoadSegment.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Common.FeatureToggles;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.V2.RoadSegments;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class RoadSegmentControllerV3
    {
        private const string CreateRoadSegmentOutlineRoute = "wegsegmenten/acties/schetsen";

        /// <summary>
        ///     Schets een wegsegment (v3).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het wegsegment gevonden is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(CreateRoadSegmentOutlineRoute, Name = nameof(CreateRoadSegmentOutlineV3))]
        [ApiOrder(ApiOrder.Road.RoadSegment.CreateOutline)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerRequestExample(typeof(CreateOutlinedRoadSegmentV2Parameters), typeof(CreateOutlinedRoadSegmentV2ParametersExamples))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(CreateRoadSegmentOutlineV3),
            Description = "Voeg een nieuw wegsegment toe aan het Wegenregister met geometriemethode 'ingeschetst'.",
            Authorize = Scopes.DvWrGeschetsteWegBeheer
        )]
        public async Task<IActionResult> CreateRoadSegmentOutlineV3(
            [FromBody] CreateOutlinedRoadSegmentV2Parameters request,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] CreateRoadSegmentOutlineV3Toggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, CreateRoadSegmentOutlineRoute)
                    .AddJsonBody(request);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken
            );

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
        }
    }
}
