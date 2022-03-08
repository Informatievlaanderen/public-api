namespace Public.Api.StreetName.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameBackOfficeController
    {
        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="streetNameProposeRequest"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="proposeStreetNameToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="201">Als de straatnaam succesvol voorgesteld is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string", "De URL van de voorgestelde straatnaam.", "")]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Voer een nieuwe straatnaam in met status \"voorgesteld\".")]
        [HttpPost("straatnamen/voorgesteld", Name = nameof(ProposeStreetName))]
        public async Task<IActionResult> ProposeStreetName(
            [FromBody] StreetNameProposeRequest streetNameProposeRequest,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ProposeStreetNameToggle proposeStreetNameToggle,
            CancellationToken cancellationToken = default)
        {
            if (!proposeStreetNameToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            IRestRequest BackendRequest() => CreateBackendRequestWithJsonBody(
                "straatnamen/voorgesteld",
                streetNameProposeRequest,
                Method.POST);

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
