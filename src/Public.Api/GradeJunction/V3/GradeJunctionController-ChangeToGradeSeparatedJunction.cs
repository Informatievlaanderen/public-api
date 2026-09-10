namespace Public.Api.GradeJunction.V3
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
    using RoadRegistry.BackOffice.Api.V2.GradeJunctions;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class GradeJunctionControllerV3
    {
        private const string ChangeToGradeSeparatedJunctionRoute = "gelijkgrondsekruisingen/{id}/acties/wijzigen/naarongelijkgrondsekruising";

        /// <summary>
        ///     Wijzig een gelijkgrondse kruising naar een ongelijkgrondse kruising. (v3)
        /// </summary>
        /// <param name="id">De identificator van de gelijkgrondse kruising.</param>
        /// <param name="request"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het verzoek aanvaard is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als de gelijkgrondse kruising niet gevonden kan worden.</response>
        /// <response code="410">Als de gelijkgrondse kruising is verwijderd.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost(ChangeToGradeSeparatedJunctionRoute, Name = nameof(ChangeGradeJunctionToGradeSeparatedJunctionV3))]
        [ApiOrder(ApiOrder.Road.GradeJunction.ChangeToGradeSeparatedJunction)]
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
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GradeJunctionNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(GradeJunctionGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerRequestExample(typeof(ChangeToGradeSeparatedJunctionV2Parameters), typeof(ChangeToGradeSeparatedJunctionV2ParametersExamples))]
        [SwaggerAuthorizeOperation(
            OperationId = nameof(ChangeGradeJunctionToGradeSeparatedJunctionV3),
            Description = "Wijzig een gelijkgrondse kruising naar een ongelijkgrondse kruising.",
            Authorize = Scopes.DvWrGeschetsteWegBeheer
        )]
        public async Task<IActionResult> ChangeGradeJunctionToGradeSeparatedJunctionV3(
            [FromRoute] int id,
            [FromBody] ChangeToGradeSeparatedJunctionV2Parameters request,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ChangeGradeJunctionToGradeSeparatedJunctionV3Toggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Post, ChangeToGradeSeparatedJunctionRoute)
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment)
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
