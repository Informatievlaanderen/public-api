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
        /// Vervolledig een tickets (v2).
        /// </summary>
        /// <param name="originator"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het ticket vervolledigd werd.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost("tickets/create/{originator}", Name = nameof(CreateTicket))]
        [ApiKeyAuth("tickets")]
        [ApiOrder(ApiOrder.TicketingService + 3)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Guid))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> CreateTicket(
            string originator,
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

            RestRequest BackendRequest() => CreateBackendCreateRequest(originator);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private static RestRequest CreateBackendCreateRequest(string originator)
        {
            var request = new RestRequest("tickets/{ticketId}/complete");
            request.AddParameter("originator", originator, ParameterType.UrlSegment);
            return request;
        }
    }
}
