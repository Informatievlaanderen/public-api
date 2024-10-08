namespace Public.Api.StreetName.Oslo
{
    using System;
    using System.Collections.Generic;
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
    using RestSharp;
    using StreetNameRegistry.Api.Oslo.StreetName.List;
    using StreetNameRegistry.Api.Oslo.StreetName.Query;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameOsloController
    {
        /// <summary>
        /// Vraag een lijst met straatnamen op (v2).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt. De offset is echter beperkt tot 1000000, indien meer data dient ingelezen te worden is het gebruik van extra filters aangewezen op de service of verwijzen we naar de <a href="https://basisregisters.vlaanderen.be/producten/grar" target="_blank" >downloadproducten van het gebouwen- en adressenregister</a> (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, naam-nl, naam-fr, naam-de, naam-en).</param>
        /// <param name="straatnaam">Filter op de naam van de straatnaam (exact) (optioneel).</param>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van de straatnaam (exact) (optioneel).</param>
        /// <param name="niscode">Filter op de NIS-code van de straatnaam (exact) (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van de straatnaam (exact) (optioneel). \
        /// `"voorgesteld"` `"inGebruik"` `"gehistoreerd"` `"afgekeurd"`
        /// </param>
        /// <param name="gewest">Filter op het gewest van de straatnaam (exact) (optioneel). \
        /// `"vlaams"`
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen", Name = nameof(ListStreetNamesV2))]
        [ApiOrder(ApiOrder.StreetName.V2 + 2)]
        [ProducesResponseType(typeof(StreetNameListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
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
            [FromQuery] string gewest,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<StreetNameOptionsV2> responseOptions,
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
                straatnaam,
                gemeentenaam,
                niscode,
                status,
                gewest);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static RestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            Taal language,
            string sort,
            string streetNameName,
            string municipalityName,
            string nisCode,
            string status,
            string? gewest)
        {
            var filter = new StreetNameFilter
            {
                StreetNameName = streetNameName,
                MunicipalityName = municipalityName,
                NisCode = nisCode,
                Status = status,
                IsInFlemishRegion = gewest?.Equals("vlaams", StringComparison.OrdinalIgnoreCase)
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
