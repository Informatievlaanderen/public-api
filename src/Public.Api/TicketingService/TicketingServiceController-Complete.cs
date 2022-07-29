namespace Public.Api.TicketingService
{
    using System;
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
        /// Vervolledig een ticket (v1).
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="ticketResult"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het ticket vervolledigd werd.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPut("tickets/{ticketId}/complete", Name = nameof(CompleteTicket))]
        [ProducesResponseType(typeof(Task), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Task))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> CompleteTicket(
            Guid ticketId,
            TicketResult? ticketResult,
            [FromServices] IActionContextAccessor actionContextAccessor,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendCompleteRequest(ticketId, ticketResult);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private static RestRequest CreateBackendCompleteRequest(Guid ticketId, TicketResult? ticketResult)
        {
            var request = new RestRequest("tickets/{ticketId/complete}");
            request.AddParameter("ticketId", ticketId, ParameterType.UrlSegment);
            request.AddParameter("ticketResult", ticketResult, ParameterType.RequestBody);
            return request;
        }
    }
}
