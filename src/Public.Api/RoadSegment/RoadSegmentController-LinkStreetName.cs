namespace Public.Api.RoadSegment
{
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
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
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentController
    {
        private const string LinkRoadSegmentStreetNameRoute = "wegsegmenten/{id}/acties/straatnaamkoppelen";

        /// <summary>
        ///     Koppel een straatnaam aan een wegsegment (v1).
        /// </summary>
        /// <param name="id">Identificator van het wegsegment.</param>
        /// <param name="request"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als het wegsegment niet gevonden kan worden.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(LinkRoadSegmentStreetNameRoute, Name = nameof(LinkRoadSegmentStreetName))]
        [ApiOrder(ApiOrder.Road.RoadSegment + 8)]
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
        [SwaggerRequestExample(typeof(PostLinkStreetNameParameters), typeof(PostLinkStreetNameParametersExamples))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(LinkRoadSegmentStreetName),
            Description = "Koppel een linker- en/of rechterstraatnaam met status `voorgesteld` of `inGebruik` aan een wegsegment waaraan momenteel geen linker- en/of rechterstraatnaam gekoppeld werd.",
            Authorize = Scopes.DvWrAttribuutWaardenBeheer
        )]
        public async Task<IActionResult> LinkRoadSegmentStreetName(
            [FromRoute] string id,
            [FromBody] PostLinkStreetNameParameters request,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] LinkRoadSegmentStreetNameToggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, LinkRoadSegmentStreetNameRoute)
                    .AddJsonBody(request)
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment);

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
