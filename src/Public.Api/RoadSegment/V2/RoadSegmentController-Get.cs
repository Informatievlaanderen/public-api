namespace Public.Api.RoadSegment.V2
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.ProblemDetailsExceptionMappings;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.RoadSegments.V1;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class RoadSegmentControllerV2
    {
        private const string GetRoadSegmentRoute = "wegsegmenten/{id}";

        /// <summary>
        ///     Vraag een wegsegment op (v2).
        /// </summary>
        /// <param name="id">De identificator van het wegsegment.</param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het wegsegment gevonden is.</response>
        /// <response code="404">Als het wegsegment niet gevonden kan worden, of als de inwinning ervan compleet is - dan verwijst de Link-header naar het wegsegment in v3.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet(GetRoadSegmentRoute, Name = nameof(GetRoadSegmentV2))]
        [ApiOrder(ApiOrder.Road.RoadSegment.Get)]
        [ProducesResponseType(typeof(GetRoadSegmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status404NotFound, HeaderNames.Link, JsonSchemaType.String, "De URL van het wegsegment in v3.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetRoadSegmentResponseResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadSegmentNotFoundResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerOperation(OperationId = nameof(GetRoadSegmentV2))]
        public async Task<IActionResult> GetRoadSegmentV2(
            [FromRoute] int id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] GetRoadSegmentToggle featureToggle,
            CancellationToken cancellationToken)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat();

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, GetRoadSegmentRoute)
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment);

            string? successorVersionLink = null;

            try
            {
                var value = await GetFromBackendWithBadRequestAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    problemDetailsHelper,
                    cancellationToken: cancellationToken,
                    inspectNotOkResponse: response => successorVersionLink = response.StatusCode == HttpStatusCode.NotFound
                        ? response.Headers?.FirstOrDefault(header => string.Equals(header.Name, HeaderNames.Link, StringComparison.OrdinalIgnoreCase))?.Value?.ToString()
                        : null
                );

                return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
            }
            catch (NotFoundException exception) when (!string.IsNullOrWhiteSpace(successorVersionLink))
            {
                // A road segment whose inwinning is complete is only served by v3: not found here, as any other, but with
                // a link to where it lives now. Answered rather than thrown, since handling the exception would clear the
                // response headers, the link with them.
                Response.Headers[HeaderNames.Link] = successorVersionLink;

                return new ObjectResult(new NotFoundExceptionMapping().MapException(HttpContext, exception, problemDetailsHelper))
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
        }
    }
}
