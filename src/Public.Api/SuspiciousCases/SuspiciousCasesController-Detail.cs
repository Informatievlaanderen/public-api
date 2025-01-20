namespace Public.Api.SuspiciousCases
{
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.Detail;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class SuspiciousCasesController
    {
        /// <summary>
        /// Vraag een verdacht geval op.
        /// </summary>
        /// <param name="type">Het type van het verdacht geval.</param>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt. De offset is echter beperkt tot 1000000, indien meer data dient ingelezen te worden is het gebruik van extra filters aangewezen op de service of verwijzen we naar de <a href="https://basisregisters.vlaanderen.be/producten/grar" target="_blank" >downloadproducten van het gebouwen- en adressenregister</a> (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="niscode">Filter op de NIS-code van het verdacht geval (exact) (optioneel).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="responseOptions"></param>
        /// <param name="suspiciousCasesToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het verdacht geval gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als u niet geauthenticeerd bent om deze actie uit te voeren.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("verdachte-gevallen/{type}", Name = nameof(DetailSuspiciousCases))]
        [ApiOrder(ApiOrder.SuspiciousCases.V2 + 2)]
        [ProducesResponseType(typeof(SuspiciousCasesDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SuspiciousCasesDetailResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedOAuthResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> DetailSuspiciousCases(
            [FromRoute] int type,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string? niscode,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] IOptions<SuspiciousCasesOptionsV2> responseOptions,
            [FromServices] DetailSuspiciousCasesToggle suspiciousCasesToggle,
            CancellationToken cancellationToken = default)
        {
            if (!suspiciousCasesToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendDetailRequest(
                type,
                offset,
                limit,
                niscode,
                actionContextAccessor);

            var nextUrl = responseOptions.Value.SuspiciousCasesTypeNextUrl.Replace("{type}", type.ToString());

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);


            return BackendListResponseResult.Create(value, Request.Query, nextUrl);
        }

        private static RestRequest CreateBackendDetailRequest(
            int type,
            int? offset,
            int? limit,
            string? nisCode,
            IActionContextAccessor actionContextAccessor)
        {
            var filter = new SuspiciousCasesDetailFilter
            {
                NisCode = nisCode,
            };

            return new RestRequest("verdachte-gevallen/{type}")
                .AddParameter("type", type, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddHeaderAuthorization(actionContextAccessor);
        }
    }
}
