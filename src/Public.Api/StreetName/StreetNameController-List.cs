namespace Public.Api.StreetName
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
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
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using StreetNameRegistry.Api.Legacy.StreetName.Query;
    using StreetNameRegistry.Api.Legacy.StreetName.Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameController
    {
        /// <summary>
        /// Vraag een lijst met actieve straatnamen op.
        /// </summary>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, naam-nl, naam-fr, naam-de, naam-en).</param>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de straatnamen (exact).</param>
        /// <param name="naamNl">Filter op het Nederlandse deel van de straatnaam (bevat).</param>
        /// <param name="naamFr">Filter op het Franse deel van de straatnaam (bevat).</param>
        /// <param name="naamDe">Filter op het Duitse deel van de straatnaam (bevat).</param>
        /// <param name="naamEn">Filter op het Engelse deel van de straatnaam (bevat).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen")]
        [ProducesResponseType(typeof(List<StreetNameListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> ListStreetNames(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeenteNaam,
            [FromQuery] string naamNl,
            [FromQuery] string naamFr,
            [FromQuery] string naamDe,
            [FromQuery] string naamEn,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<StreetNameOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await ListStreetNamesWithFormat(
                null,
                offset,
                limit,
                sort,
                gemeenteNaam,
                naamNl,
                naamFr,
                naamDe,
                naamEn,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een lijst met actieve straatnamen op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, naam-nl, naam-fr, naam-de, naam-en).</param>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de straatnamen (exact).</param>
        /// <param name="naamNl">Filter op het Nederlandse deel van de straatnaam (bevat).</param>
        /// <param name="naamFr">Filter op het Franse deel van de straatnaam (bevat).</param>
        /// <param name="naamDe">Filter op het Duitse deel van de straatnaam (bevat).</param>
        /// <param name="naamEn">Filter op het Engelse deel van de straatnaam (bevat).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen.{format}")]
        [ProducesResponseType(typeof(List<StreetNameListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> ListStreetNamesWithFormat(
            [FromRoute] string format,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeenteNaam,
            [FromQuery] string naamNl,
            [FromQuery] string naamFr,
            [FromQuery] string naamDe,
            [FromQuery] string naamEn,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<StreetNameOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            var taal = Taal.NL;

            void HandleBadRequest(HttpStatusCode statusCode)
            {
                switch (statusCode)
                {
                    case HttpStatusCode.NotAcceptable:
                        throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);

                    case HttpStatusCode.BadRequest:
                        throw new ApiException("Ongeldige vraag.", StatusCodes.Status400BadRequest);
                }
            }

            IRestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                gemeenteNaam,
                naamNl,
                naamFr,
                naamDe,
                naamEn);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/streetname-list:{taal}");

            var value = await (CacheToggle.FeatureEnabled
                ? GetFromCacheThenFromBackendAsync(format, BackendRequest, cacheKey, Request.GetTypedHeaders(), HandleBadRequest, cancellationToken)
                : GetFromBackendAsync(format, BackendRequest, Request.GetTypedHeaders(), HandleBadRequest, cancellationToken));

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        protected IRestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            Taal taal,
            string sort,
            string municipalityName,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish)
        {
            var filter = new StreetNameFilter
            {
                MunicipalityName = municipalityName,
                NameDutch = nameDutch,
                NameFrench = nameFrench,
                NameGerman = nameGerman,
                NameEnglish = nameEnglish
            };

            // id, naam-nl, naam-fr, naam-de, naam-en
            var sortMapping = new Dictionary<string, string>
            {
                { "Id", "OsloId" },
                { "NaamNl", "NameDutch" },
                { "Naam-Nl", "NameDutch" },
                { "NaamEn", "NameEnglish" },
                { "Naam-En", "NameEnglish" },
                { "NaamFr", "NameFrench" },
                { "Naam-Fr", "NameFrench" },
                { "NaamDe", "NameGerman" },
                { "Naam-De", "NameGerman" },
            };

            return new RestRequest("straatnamen?taal={taal}")
                .AddParameter("taal", taal, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
