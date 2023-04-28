namespace Public.Api.RoadSegment
{
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
    using RoadRegistry.BackOffice.Api.RoadSegments;
    using RoadRegistry.BackOffice.Api.RoadSegments.Parameters;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentController
    {
        private const string ChangeRoadSegmentOutlineGeometryRoute = "wegsegmenten/{id}/acties/wijzigen/schetsgeometrie";

        /// <summary>
        ///     Wijzig de geometrie van een ingeschetst wegsegment.
        /// </summary>
        /// <param name="id">Identificator van het wegsegment.</param>
        /// <param name="request"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het wegsegment gevonden is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als het wegsegment niet gevonden kan worden.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(ChangeRoadSegmentOutlineGeometryRoute, Name = nameof(ChangeRoadSegmentOutlineGeometry))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadSegmentNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerRequestExample(typeof(PostChangeOutlineGeometryParameters), typeof(PostChangeOutlineGeometryParametersExamples))]
        [SwaggerOperation(OperationId = nameof(ChangeRoadSegmentOutlineGeometry), Description = "Wijzig de geometrie van een wegsegment met geometriemethode 'ingeschetst'.")]
        public async Task<IActionResult> ChangeRoadSegmentOutlineGeometry(
            [FromRoute] string id,
            [FromBody] PostChangeOutlineGeometryParameters request,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ChangeRoadSegmentOutlineGeometryToggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest()
            {
                return CreateBackendRequestWithJsonBody(
                        ChangeRoadSegmentOutlineGeometryRoute,
                        request,
                        Method.Post)
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment)
                    .AddHeaderAuthorization(actionContextAccessor);
            }

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
