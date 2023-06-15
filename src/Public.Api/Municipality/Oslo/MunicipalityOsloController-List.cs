namespace Public.Api.Municipality.Oslo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using MunicipalityRegistry.Api.Oslo.Municipality.Query;
    using MunicipalityRegistry.Api.Oslo.Municipality.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class MunicipalityOsloController
    {
        /// <summary>
        /// Vraag een lijst met gemeenten op (v2).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (niscode, naam, naam-nl, naam-fr, naam-de, naam-en).</param>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van de gemeente (exact) (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van de gemeente (exact) (optioneel). <br />
        /// `"inGebruik"` `"gehistoreerd"` `"voorgesteld"`
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gemeenten gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [ApiOrder(ApiOrder.Municipality.V2 + 2)]
        [HttpGet("gemeenten", Name = nameof(ListMunicipalitiesV2))]
        [ProducesResponseType(typeof(MunicipalityListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListMunicipalitiesV2(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeentenaam,
            [FromQuery] string status,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptionsV2> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            const Taal taal = Taal.NL;

            var isFlemishRegion = GetIsFlemishRegionQueryParameter();

            RestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                gemeentenaam,
                sort,
                status,
                isFlemishRegion);

            var value = await GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private bool GetIsFlemishRegionQueryParameter()
        {
            var isFlemishRegion = false;
            var isFlemishRegionParameterName = "isFlemishRegion";
            if (Request.Query.ContainsKey(isFlemishRegionParameterName))
            {
                bool.TryParse(Request.Query[isFlemishRegionParameterName].First(), out isFlemishRegion);
            }

            return isFlemishRegion;
        }

        private static RestRequest CreateBackendListRequest(int? offset,
            int? limit,
            Taal language,
            string municipalityName,
            string sort,
            string status,
            bool isFlemishRegion)
        {
            var filter = new MunicipalityListFilter
            {
                MunicipalityName = municipalityName,
                Status = status,
                IsFlemishRegion = isFlemishRegion
            };

            // niscode, naam, naam-nl, naam-fr, naam-de, naam-en
            var sortMapping = new Dictionary<string, string>
            {
                { "NisCode", "NisCode" },
                { "Naam", "DefaultName" },
                { "NaamNl", "NameDutch" },
                { "Naam-Nl", "NameDutch" },
                { "NaamEn", "NameEnglish" },
                { "Naam-En", "NameEnglish" },
                { "NaamFr", "NameFrench" },
                { "Naam-Fr", "NameFrench" },
                { "NaamDe", "NameGerman" },
                { "Naam-De", "NameGerman" }
            };

            return new RestRequest("gemeenten?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
