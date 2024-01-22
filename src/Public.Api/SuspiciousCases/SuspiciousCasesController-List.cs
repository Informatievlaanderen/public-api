namespace Public.Api.SuspiciousCases
{
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.List;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class SuspiciousCasesController
    {
        /// <summary>
        /// Vraag een lijst met verdachte gevallen op.
        /// </summary>
        /// <param name="nisCode">Filter op de NIS-code van het verdachte geval.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="suspiciousCasesToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met verdachte gevallen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("verdachte-gevallen", Name = nameof(ListSuspiciousCases))]
        [ApiOrder(ApiOrder.SuspiciousCases + 1)]
        [ProducesResponseType(typeof(SuspiciousCasesListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)] // TODO: can this be removed
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)] // TODO: can this be removed
        public async Task<IActionResult> ListSuspiciousCases(
            [FromQuery] string nisCode,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] GetSuspiciousCasesToggle suspiciousCasesToggle,
            CancellationToken cancellationToken = default)
        {
            if (!suspiciousCasesToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendListRequest(
                nisCode,
                actionContextAccessor);

            var value = await  GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken);

            return new BackendResponseResult(value);
        }

        private static RestRequest CreateBackendListRequest(
            string nisCode,
            IActionContextAccessor actionContextAccessor)
        {
            var filter = new SuspiciousCasesListFilter
            {
                NisCode = nisCode,
            };

            return new RestRequest("verdachte-gevallen")
                .AddFiltering(filter)
                .AddHeaderAuthorization(actionContextAccessor);
        }
    }
}