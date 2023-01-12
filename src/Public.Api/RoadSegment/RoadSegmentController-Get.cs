namespace Public.Api.RoadSegment;

using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Infrastructure;
using RestSharp;
using RoadRegistry.BackOffice.Api.RoadSegments;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Annotations;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

public partial class RoadSegmentController
{
    public const string GetRoadSegmentRoute = "wegsegmenten/{id}";

    /// <summary>
    /// Vraag een wegsegment op.
    /// </summary>
    /// <param name="id">De identificator van het wegsegment.</param>
    /// <param name="actionContextAccessor"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="featureToggle"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Als het wegsegment gevonden is.</response>
    /// <response code="404">Als het wegsegment niet gevonden kan worden.</response>
    /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet(GetRoadSegmentRoute, Name = nameof(Get))]
    [ProducesResponseType(typeof(GetRoadSegmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetRoadSegmentResponseResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RoadSegmentNotFoundResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerOperation(OperationId = "GetRoadSegment")]
    public async Task<IActionResult> Get(
        [FromRoute] int id,
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        [FromServices] RoadSegmentGetToggle featureToggle,
        CancellationToken cancellationToken)
    {
        if (!featureToggle.FeatureEnabled)
        {
            return NotFound();
        }

        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

        RestRequest BackendRequest()
        {
            return new RestRequest(GetRoadSegmentRoute)
                {
                    Method = Method.Get
                }
                .AddParameter(nameof(id), id, ParameterType.UrlSegment);
        }

        var value = await GetFromBackendWithBadRequestAsync(
            contentFormat.ContentType,
            BackendRequest,
            CreateDefaultHandleBadRequest(),
            problemDetailsHelper,
            cancellationToken: cancellationToken
        );

        return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
    }
}
