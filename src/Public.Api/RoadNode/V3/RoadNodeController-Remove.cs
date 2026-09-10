namespace Public.Api.RoadNode.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Common.FeatureToggles;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.V2.RoadNodes;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadNodeControllerV3
    {
        private const string RemoveRoadNodeRoute = "wegknopen/{id}/acties/verwijderen";

        /// <summary>
        ///     Verwijder een wegknoop, waarbij de aansluitende wegsegmenten samengevoegd worden. (v3)
        /// </summary>
        /// <param name="id">De identificator van de wegknoop.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het verzoek aanvaard is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als de wegknoop niet gevonden kan worden.</response>
        /// <response code="410">Als de wegknoop is verwijderd.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(RemoveRoadNodeRoute, Name = nameof(RemoveRoadNodeV3))]
        [ApiOrder(ApiOrder.Road.RoadNode.Remove)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadNodeNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(RoadNodeGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(RemoveRoadNodeV3),
            Description = "Verwijder een wegknoop. De aansluitende wegsegmenten worden hierbij samengevoegd: twee bij een validatieknoop, paarsgewijs bij een echte knoop met precies vier aansluitende wegsegmenten.",
            Authorize = Scopes.DvWrGeschetsteWegBeheer
        )]
        public async Task<IActionResult> RemoveRoadNodeV3(
            [FromRoute] int id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RemoveRoadNodeV3Toggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, RemoveRoadNodeRoute)
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
