namespace Public.Api.Building
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using BuildingRegistry.Api.Legacy.Building.List;
    using BuildingRegistry.Api.Legacy.Building.Query;
    using Common.Infrastructure;
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

    public partial class BuildingController
    {
        /// <summary>
        /// Vraag een lijst met gebouwen op (v1).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id).</param>
        /// <param name="status">Filter op de status van het gebouw (exact) (optioneel).<br/>
        /// `"gepland"` `"inAanbouw"` `"gerealiseerd"` `"gehistoreerd"` `"nietGerealiseerd"`
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gebouwen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen", Name = nameof(ListBuildings))]
        [ApiOrder(ApiOrder.Building.V1 + 2)]
        [ProducesResponseType(typeof(BuildingListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingListResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListBuildings(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string status,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<BuildingOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            const Taal taal = Taal.NL;

            RestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                status);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.GebouwVolgendeUrl);
        }

        private static RestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            Taal language,
            string sort,
            string status)
        {
            var filter = new BuildingFilter
            {
                Status = status
            };

            var sortMapping = new Dictionary<string, string>
            {
                { "Id", "PersistentLocalId" },
            };

            return new RestRequest("gebouwen?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
