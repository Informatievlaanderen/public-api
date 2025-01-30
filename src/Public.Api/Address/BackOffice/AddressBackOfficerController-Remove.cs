namespace Public.Api.Address.BackOffice
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
    using AddressRegistry.Api.Oslo.Address.Detail;
    using Common.FeatureToggles;
    using Common.Infrastructure.Extensions;
    using Infrastructure.Swagger;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressBackOfficeController
    {
        public const string RemoveAddressRoute = "adressen/{objectId}/acties/verwijderen";

        /// <summary>
        /// Verwijder een adres (v2).
        /// </summary>
        /// <param name="objectId">Identificator van het adres.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van het adres (optioneel).</param>
        /// <param name="removeAddressToggle"></param>
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
        [ApiOrder(ApiOrder.Address.Edit + 5)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(AddressNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerOperation(Description = "Het adres wordt verwijderd uit het adressenregister. Gekoppelde busnummers met status `voorgesteld`, `inGebruik`, `afgekeurd` en `gehistoreerd` worden mee verwijderd. Als er een gebouweenheid of perceel gekoppeld is aan het adres, wordt deze koppeling ook verwijderd.")]
        [HttpPost(RemoveAddressRoute, Name = nameof(RemoveAddress))]
        public async Task<IActionResult> RemoveAddress(
            [FromRoute] int objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RemoveAddressToggle removeAddressToggle,
            [FromHeader(Name = HeaderNames.IfMatch)] string? ifMatch,
            CancellationToken cancellationToken = default)
        {
            if (!removeAddressToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => new RestRequest(RemoveAddressRoute, Method.Post)
                .AddParameter("objectId", objectId, ParameterType.UrlSegment)
                .AddHeaderIfMatch(ifMatch)
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
