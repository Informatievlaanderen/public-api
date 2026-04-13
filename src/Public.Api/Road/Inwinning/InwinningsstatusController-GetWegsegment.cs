namespace Public.Api.Road.Inwinning
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Inwinning;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class InwinningsstatusControllerV2
    {
        /// <summary>
        ///     Vraag de inwinningsstatus van een wegsegment op (v2).
        /// </summary>
        /// <param name="id">De identificator van het wegsegment.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het wegsegment gevonden is.</response>
        /// <response code="404">Als het wegsegment niet gevonden kan worden.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [AllowAnonymous]
        [HttpGet("wegen/inwinningsstatus/wegsegment/{id}", Name = nameof(GetWegsegmentInwinningsstatus))]
        [ApiOrder(ApiOrder.Road.Inwinningsstatus)]
        [ProducesResponseType(typeof(WegsegmentInwinningsstatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WegsegmentInwinningsstatusResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(InwinningsstatusV2NotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerOperation(OperationId = nameof(GetWegsegmentInwinningsstatus))]
        public async Task<ActionResult> GetWegsegmentInwinningsstatus(
            [FromRoute] int id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadInwinningsstatusWegsegmentToggle featureToggle,
            CancellationToken cancellationToken = default)
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

            return new BackendResponseResult(response);

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, $"inwinningsstatus/wegsegment/{id}");
        }
    }
}
