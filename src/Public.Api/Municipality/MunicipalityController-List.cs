namespace Public.Api.Municipality
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using MunicipalityRegistry.Api.Legacy.Municipality.Query;
    using MunicipalityRegistry.Api.Legacy.Municipality.Responses;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;

    public partial class MunicipalityController
    {
        /// <summary>
        /// Vraag een lijst met actieve gemeenten op.
        /// </summary>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="nisCode">Filter op de NIS Code van de gemeente.</param>
        /// <param name="naamNl">Filter op het Nederlandse deel van de gemeente (bevat).</param>
        /// <param name="naamFr">Filter op het Franse deel van de gemeente (bevat).</param>
        /// <param name="naamDe">Filter op het Duitse deel van de gemeente (bevat).</param>
        /// <param name="naamEn">Filter op het Engelse deel van de gemeente (bevat).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gemeenten gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gemeenten")]
        [ProducesResponseType(typeof(List<MunicipalityListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> List(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string nisCode,
            [FromQuery] string naamNl,
            [FromQuery] string naamFr,
            [FromQuery] string naamDe,
            [FromQuery] string naamEn,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await List(
                null,
                offset,
                limit,
                nisCode,
                naamNl,
                naamFr,
                naamDe,
                naamEn,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een lijst met actieve gemeenten op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="nisCode">Filter op de NIS Code van de gemeente.</param>
        /// <param name="naamNl">Filter op het Nederlandse deel van de gemeente (bevat).</param>
        /// <param name="naamFr">Filter op het Franse deel van de gemeente (bevat).</param>
        /// <param name="naamDe">Filter op het Duitse deel van de gemeente (bevat).</param>
        /// <param name="naamEn">Filter op het Engelse deel van de gemeente (bevat).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gemeenten gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gemeenten.{format}")]
        [ProducesResponseType(typeof(List<MunicipalityListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "CorrelationId", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> List(
            [FromRoute] string format,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string nisCode,
            [FromQuery] string naamNl,
            [FromQuery] string naamFr,
            [FromQuery] string naamDe,
            [FromQuery] string naamEn,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            offset = offset ?? 0;
            limit = limit ?? DefaultLimit;
            Taal? taal = Taal.NL;

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

            RestRequest BackendRequest() => CreateBackendListRequest(
                offset.Value,
                limit.Value,
                taal.Value,
                nisCode,
                naamNl,
                naamFr,
                naamDe,
                naamEn);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/municipality-list:{taal}");

            var value = await (CacheToggle.FeatureEnabled
                ? GetFromCacheThenFromBackendAsync(format, BackendRequest, cacheKey, Request.GetTypedHeaders(), HandleBadRequest, cancellationToken)
                : GetFromBackendAsync(format, BackendRequest, Request.GetTypedHeaders(), HandleBadRequest, cancellationToken));

            return  BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        protected RestRequest CreateBackendListRequest(
            int offset,
            int limit,
            Taal taal,
            string nisCode,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish)
        {
            var request = new RestRequest("gemeenten?taal={taal}");
            request.AddHeader(AddPaginationExtension.HeaderName, $"{offset},{limit}");
            request.AddParameter("taal", taal, ParameterType.UrlSegment);

            var filter = new MunicipalityFilter
            {
                NisCode = nisCode,
                NameDutch = nameDutch,
                NameFrench = nameFrench,
                NameGerman = nameGerman,
                NameEnglish = nameEnglish
            };

            request.AddHeader(ExtractFilteringRequestExtension.HeaderName, JsonConvert.SerializeObject(filter));

            return request;
        }
    }
}
