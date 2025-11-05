namespace Public.Api.Address.IntegrationDb
{
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.Api.Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class AddressIntegrationDbController
    {
        private const string CorrectDerivedFromBuildingUnitPositionsRoute = "adressen/acties/corrigeren/afgeleid-van-gebouweenheid-posities";

        /// <summary>
        /// Corrigeer adres posities met methode Afgeleid en specificatie Gebouweenheid (v2).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als u niet geauthenticeerd bent om deze actie uit te voeren.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="404">Als het adres niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(CorrigerenAfgeleidVanGebouwEenhedenRequest), typeof(CorrigerenAfgeleidVanGebouwEenhedenRequestExamples))]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(CorrigerenAfgeleidVanGebouwEenhedenResponseExample))]
        [SwaggerOperation(Description = "Corrigeer adres posities met methode Afgeleid en specificatie Gebouweenheid.")]
        [HttpPost(CorrectDerivedFromBuildingUnitPositionsRoute, Name = nameof(CorrectDerivedFromBuildingUnitPositions))]
        public async Task<IActionResult> CorrectDerivedFromBuildingUnitPositions(
            [FromBody] CorrigerenAfgeleidVanGebouwEenhedenRequest? request,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() =>
                CreateBackendRequestWithJsonBody($"{BackOfficeVersion}/adressen/corrigeren/afgeleid-van-gebouweenheid-posities", request, Method.Post)
                    .AddHeaderAuthorization(actionContextAccessor);

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
