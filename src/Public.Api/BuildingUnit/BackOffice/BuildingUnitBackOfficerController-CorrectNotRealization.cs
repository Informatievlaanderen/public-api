namespace Public.Api.BuildingUnit.BackOffice
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.Legacy.Abstractions.BuildingUnit.Responses;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingUnitBackOfficeController
    {
        /// <summary>
        /// Corrigeer de niet realisering van een gebouweenheid.
        /// </summary>
        /// <param name="objectId">Identificator van de gebouweenheid.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="correctBuildingUnitNotRealizationToggle"></param>
        /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de gebouweenheid (optioneel).</param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het ticket succesvol is aangemaakt.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="404">Als de gebouweenheid niet gevonden kan worden.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ApiOrder(ApiOrder.BuildingUnit.Edit + 5)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "location", "string", "De URL van het aangemaakte ticket.")]
        [SwaggerResponseHeader(StatusCodes.Status202Accepted, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BuildingUnitNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Correctie van de gebouweenheidstatus van `nietGerealiseerd` naar `gepland`. Er wordt automatisch een gemeenschappelijkDeel aangemaakt vanaf dat er 2 gebouweenheden met status `gepland of `gerealiseerd` aan een gebouw gekoppeld zijn. De status van het gemeenschappelijKDeel is `gepland` wanneer het gebouw status `gepland` is. De status van het gemeenschappelijkDeel is `gerealiseerd` wanneer het gebouw status `gerealiseerd` is.")]
        [HttpPost("gebouweenheden/{objectId}/acties/corrigeren/nietrealisering", Name = nameof(CorrectBuildingUnitNotRealization))]
        public async Task<IActionResult> CorrectBuildingUnitNotRealization(
            [FromRoute] int objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] CorrectBuildingUnitNotRealizationToggle correctBuildingUnitNotRealizationToggle,
            [FromHeader(Name = HeaderNames.IfMatch)] string? ifMatch,
            CancellationToken cancellationToken = default)
        {
            if (!correctBuildingUnitNotRealizationToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest()
            {
                var request = new RestRequest("gebouweenheden/{persistentLocalId}/acties/corrigeren/nietrealisering", Method.Post);
                request.AddParameter("persistentLocalId", objectId, ParameterType.UrlSegment);

                if (ifMatch is not null)
                {
                    request.AddHeader(HeaderNames.IfMatch, ifMatch);
                }

                return request;
            }

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
