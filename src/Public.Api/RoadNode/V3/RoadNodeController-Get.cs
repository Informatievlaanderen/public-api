namespace Public.Api.RoadNode.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.V2.RoadNodes;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadNodeControllerV3
    {
        private const string GetRoadNodeRoute = "wegknopen/{id}";

        /// <summary>
        ///     Vraag een wegknoop op (v3).
        /// </summary>
        /// <param name="id">De identificator van het wegknoop.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het wegknoop gevonden is.</response>
        /// <response code="404">Als het wegknoop niet gevonden kan worden.</response>
        /// <response code="410">Als het wegknoop verwijderd is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet(GetRoadNodeRoute, Name = nameof(GetRoadNodeV3))]
        [ApiOrder(ApiOrder.Road.RoadNode.Get)]
        [ProducesResponseType(typeof(WegknoopV2Detail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WegknoopV2DetailResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadNodeNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(RoadNodeGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerOperation(OperationId = nameof(GetRoadNodeV3))]
        public async Task<IActionResult> GetRoadNodeV3(
            [FromRoute] int id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] GetRoadNodeV3Toggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, GetRoadNodeRoute)
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken
            );

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }
    }
}
