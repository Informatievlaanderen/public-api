namespace Public.Api.Tickets
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class TicketingServiceController
    {
        /// <summary>
        /// Verwijder een ticket (v2).
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het ticket verwijderd werd.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpDelete("tickets/{ticketId}", Name = nameof(DeleteTicket))]
        [ApiKeyAuth("tickets")]
        [ApiOrder(ApiOrder.TicketingService + 6)]
        [ProducesResponseType(typeof(Task), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Task))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> DeleteTicket(
            Guid ticketId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            CancellationToken cancellationToken = default)
        {
            if (!_ticketingToggle.FeatureEnabled)
            {
                return NotFound();
            }

            if (actionContextAccessor.ActionContext == null)
            {
                return BadRequest();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendDeleteRequest(ticketId);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private static RestRequest CreateBackendDeleteRequest(Guid ticketId)
        {
            var request = new RestRequest("tickets/{ticketId}");
            request.AddParameter("ticketId", ticketId, ParameterType.UrlSegment);
            return request;
        }
    }
}
