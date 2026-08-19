namespace Public.Api.Building.V3
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using BuildingRegistry.Api.Oslo.Building.V3.List;
    using BuildingRegistry.Api.Oslo.Building.V3.Query;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Configuration;
    using Public.Api.Infrastructure.Swagger;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class BuildingOsloController
    {
        /// <summary>
        /// Vraag een lijst met gebouwen op (v3).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt. De offset is echter beperkt tot 1000000, indien meer data dient ingelezen te worden is het gebruik van extra filters aangewezen op de service of verwijzen we naar de <a href="https://basisregisters.vlaanderen.be/producten/grar" target="_blank" >downloadproducten van het gebouwen- en adressenregister</a> (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id).</param>
        /// <param name="status">Filter op de status van het gebouw (exact) (optioneel). \
        /// `"gepland"` `"inAanbouw"` `"gerealiseerd"` `"gehistoreerd"` `"nietGerealiseerd"`
        /// </param>
        /// <param name="caPaKey">Filter op de objectidentificator van het gekoppelde perceel (exact) (optioneel) (CaPaKey waarbij forward slash `/` vervangen werd door koppelteken `-`).</param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="osloV3BuildingToggle"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gebouwen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouwen", Name = nameof(ListBuildingsV3))]
        [ApiOrder(ApiOrder.Building.V3 + 2)]
        [ProducesResponseType(typeof(BuildingListOsloV3Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", JsonSchemaType.String, "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", JsonSchemaType.String, "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingListResponseOsloExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV3))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV3))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListBuildingsV3(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string status,
            [FromQuery] string? caPaKey,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IOptions<BuildingOptionsV3> responseOptions,
            [FromServices] OsloV3BuildingToggle osloV3BuildingToggle,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            if (!osloV3BuildingToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(httpContextAccessor.HttpContext!);
            const Taal taal = Taal.NL;

            RestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                status,
                caPaKey);

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
            string status,
            string? caPaKey)
        {
            var filter = new BuildingFilter
            {
                Status = status,
                CaPaKey = caPaKey
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
