namespace Public.Api.Building.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.BackOffice.Abstractions.Building.Requests;
    using BuildingRegistry.Api.Legacy.Abstractions.Building.Responses;
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

    public partial class BuildingBackOfficeController
    {
        public const string CorrectBuildingGeometryOutlineRoute = "gebouwen/{objectId}/acties/wijzigen/schetsgeometriepolygoon";

        /// <summary>
        /// Wijzig de geometrie van een geschetst gebouw.
        /// </summary>
        /// <param name="objectId">Identificator van het gebouw.</param>
        /// <param name="changeBuildingOutlineRequest"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="changeBuildingGeometryOutlineToggle"></param>
        /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van het gebouw (optioneel).</param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als het gebouw niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ApiOrder(ApiOrder.Building.Edit + 9)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BuildingNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Wijzig de geometrie van een geschetst gebouw. De gekoppelde gebouweenheden moeten in deze nieuwe geometrie liggen.")]
        [HttpPost(CorrectBuildingGeometryOutlineRoute, Name = nameof(ChangeBuildingGeometryOutline))]
        public async Task<IActionResult> ChangeBuildingGeometryOutline(
            [FromRoute] int objectId,
            [FromBody] ChangeBuildingOutlineRequest changeBuildingOutlineRequest,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ChangeBuildingGeometryOutlineToggle changeBuildingGeometryOutlineToggle,
            [FromHeader(Name = HeaderNames.IfMatch)] string? ifMatch,
            CancellationToken cancellationToken = default)
        {
            if (!changeBuildingGeometryOutlineToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest () => CreateBackendRequestWithJsonBody(CorrectBuildingGeometryOutlineRoute, changeBuildingOutlineRequest, Method.Post)
                .AddParameter("objectId", objectId, ParameterType.UrlSegment)
                .AddHeaderIfMatch(HeaderNames.IfMatch, ifMatch);

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
