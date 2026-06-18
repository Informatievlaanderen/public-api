namespace Public.Api.GradeSeparatedJunction.V3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.V2.GradeSeparatedJunctions;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class GradeSeparatedJunctionControllerV3
    {
        private const string GetGradeSeparatedJunctionRoute = "ongelijkgrondsekruisingen/{id}";

        /// <summary>
        ///     Vraag een ongelijkgrondse kruising op (v3).
        /// </summary>
        /// <param name="id">De identificator van de ongelijkgrondse kruising.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de ongelijkgrondse kruising gevonden is.</response>
        /// <response code="404">Als de ongelijkgrondse kruising niet gevonden kan worden.</response>
        /// <response code="410">Als de ongelijkgrondse kruising verwijderd is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet(GetGradeSeparatedJunctionRoute, Name = nameof(GetGradeSeparatedJunctionV3))]
        [ApiOrder(ApiOrder.Road.GradeSeparatedJunction.Get)]
        [ProducesResponseType(typeof(OngelijkgrondseKruisingV2Detail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OngelijkgrondseKruisingV2DetailResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GradeSeparatedJunctionNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(GradeSeparatedJunctionGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [SwaggerOperation(OperationId = nameof(GetGradeSeparatedJunctionV3))]
        public async Task<IActionResult> GetGradeSeparatedJunctionV3(
            [FromRoute] int id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] GetGradeSeparatedJunctionV3Toggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, GetGradeSeparatedJunctionRoute)
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
