namespace Public.Api.Address.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.BackOffice.Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressBackOfficeController
    {
        public const string ReaddressStreetNameAddressesRoute = "adressen/acties/heradresseren";

        /// <summary>
        /// Heradresseren van adressen binnen éénzelfde straatnaam of naar een andere straatnaam.
        /// </summary>
        /// <param name="readdressRequest"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="readdressStreetNameAddressesToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als u niet geauthenticeerd bent om deze actie uit te voeren.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ApiOrder(ApiOrder.Address.Edit + 8)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerRequestExample(typeof(ProposeAddressRequest), typeof(ReaddressRequestExamples))]
        [SwaggerOperation(Description = "De adressen worden in bulk hernummerd binnen dezelfde of andere straatnaam. Gekoppelde adresseerbare gebouweenheden en percelen, adrespositie en status worden van het oude adres overgenomen.")]
        [HttpPost(ReaddressStreetNameAddressesRoute, Name = nameof(ReaddressStreetNameAddresses))]
        public async Task<IActionResult> ReaddressStreetNameAddresses(
            [FromBody] ReaddressRequest readdressRequest,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ReaddressStreetNameAddressesToggle readdressStreetNameAddressesToggle,
            CancellationToken cancellationToken = default)
        {
            if (!readdressStreetNameAddressesToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendRequestWithJsonBody(
                ReaddressStreetNameAddressesRoute,
                readdressRequest,
                Method.Post).AddHeaderAuthorization(actionContextAccessor);

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
