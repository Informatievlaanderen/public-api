namespace Public.Api.StreetName.Oslo
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using RestSharp;
    using StreetNameRegistry.Api.Oslo.StreetName.Query;
    using StreetNameRegistry.Api.Oslo.StreetName.Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameOsloController
    {
        /// <summary>
        /// Vraag een lijst met straatnamen op (v2).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, naam-nl, naam-fr, naam-de, naam-en).</param>
        /// <param name="straatnaam">Filter op de naam van de straatnaam (exact) (optioneel).</param>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van de straatnaam (exact) (optioneel).</param>
        /// <param name="niscode">Filter op de NIS-code van de straatnaam (exact) (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van de straatnaam (exact) (optioneel). <br />
        /// `"voorgesteld"` `"inGebruik"` `"gehistoreerd"`
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen", Name = nameof(ListStreetNamesV2))]
        [ProducesResponseType(typeof(StreetNameListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListStreetNamesV2(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string straatnaam,
            [FromQuery] string gemeentenaam,
            [FromQuery] string niscode,
            [FromQuery] string status,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<StreetNameOptionsV2> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromServices] IsStreetNameOsloApiEnabledToggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            const Taal taal = Taal.NL;

            IRestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                straatnaam,
                gemeentenaam,
                niscode,
                status);

            var cacheKey = CreateCacheKeyForRequestQuery($"oslo/streetname-list:{taal}");
            var value = await (CacheToggle.FeatureEnabled
                ? GetFromCacheThenFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    cacheKey,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken)
                : GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken));

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendListRequest(int? offset,
            int? limit,
            Taal language,
            string sort,
            string streetNameName,
            string municipalityName,
            string nisCode,
            string status)
        {
            var filter = new StreetNameFilter
            {
                StreetNameName = streetNameName,
                MunicipalityName = municipalityName,
                NisCode = nisCode,
                Status = status
            };

            // id, naam-nl, naam-fr, naam-de, naam-en
            var sortMapping = new Dictionary<string, string>
            {
                { "Id", "PersistentLocalId" },
                { "NaamNl", "NameDutch" },
                { "Naam-Nl", "NameDutch" },
                { "NaamEn", "NameEnglish" },
                { "Naam-En", "NameEnglish" },
                { "NaamFr", "NameFrench" },
                { "Naam-Fr", "NameFrench" },
                { "NaamDe", "NameGerman" },
                { "Naam-De", "NameGerman" },
            };

            return new RestRequest("straatnamen?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
