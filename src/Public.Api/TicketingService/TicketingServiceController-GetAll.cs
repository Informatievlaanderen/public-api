namespace Public.Api.TicketingService
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using global::TicketingService.Abstractions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;

    public partial class TicketingServiceController
    {
        /// <summary>
        /// Vraag alle tickets op (v1).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als alle tickets gevonden worden.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("tickets", Name = nameof(GetTickets))]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Ticket))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> GetTickets(
            [FromServices] IActionContextAccessor actionContextAccessor,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendGetAllRequest();

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private static RestRequest CreateBackendGetAllRequest() => new RestRequest("tickets");
    }
}
