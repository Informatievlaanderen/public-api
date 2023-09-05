namespace Public.Api.RoadSegment
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AcmIdm;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.RoadSegments;
    using RoadRegistry.BackOffice.Api.RoadSegments.ChangeDynamicAttributes;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentController
    {
        private const string ChangeRoadSegmentDynamicAttributesRoute = "wegsegmenten/acties/wijzigen/dynamischeattributen";

        /// <summary>
        ///     Wijzig een dynamisch attribuut voor één of meerdere wegsegmenten (v1).
        /// </summary>
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
        [HttpPost(ChangeRoadSegmentDynamicAttributesRoute, Name = nameof(ChangeRoadSegmentDynamicAttributes))]
        [ApiOrder(ApiOrder.Road.RoadSegment + 3)]
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
        [SwaggerRequestExample(typeof(ChangeRoadSegmentDynamicAttributesParameters), typeof(ChangeRoadSegmentsDynamicAttributesParametersExamples))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(ChangeRoadSegmentDynamicAttributes),
            Description = "Dynamische attributen wijzigen van een wegsegment: wegverharding, wegbreedte en aantal rijstroken.",
            Authorize = Scopes.DvWrAttribuutWaardenBeheer
        )]
        public async Task<IActionResult> ChangeRoadSegmentDynamicAttributes(
            [FromBody] ChangeRoadSegmentDynamicAttributesParameters request,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ChangeRoadSegmentDynamicAttributesToggle featureToggle,
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
                        ChangeRoadSegmentDynamicAttributesRoute,
                        request,
                        Method.Post)
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
