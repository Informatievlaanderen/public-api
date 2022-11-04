namespace Public.Api.StreetName.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameBackOfficeController
    {
        public const string ProposeStreetNameRoute = "straatnamen/acties/voorstellen";

        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="streetNameProposeRequest"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="proposeStreetNameToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ApiOrder(ApiOrder.StreetName.Edit + 1)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Voer een nieuwe straatnaam in met status `voorgesteld` binnen een gemeente met status `voorgesteld` of `inGebruik`.")]
        [HttpPost(ProposeStreetNameRoute, Name = nameof(ProposeStreetName))]
        public async Task<IActionResult> ProposeStreetName(
            [FromBody] StreetNameProposeRequest streetNameProposeRequest,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ProposeStreetNameToggle proposeStreetNameToggle,
            CancellationToken cancellationToken = default)
        {
            if (!proposeStreetNameToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendRequestWithJsonBody(
                ProposeStreetNameRoute,
                streetNameProposeRequest,
                Method.Post);

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
