namespace Public.Api.PostalCode
{
    using System.Collections.Generic;
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
    using Newtonsoft.Json.Converters;
    using PostalRegistry.Api.Legacy.PostalInformation.Query;
    using PostalRegistry.Api.Legacy.PostalInformation.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class PostalCodeController
    {
        /// <summary>
        /// Vraag een lijst met postinfo over postcodes op.
        /// </summary>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat.</param>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de postcodes (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met postinfo over postcodes gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo")]
        [ProducesResponseType(typeof(List<PostalInformationListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PostalInformationListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListPostalCodes(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeentenaam,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<PostalOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await ListPostalCodesWithFormat(
                null,
                offset,
                limit,
                sort,
                gemeentenaam,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een lijst met postinfo over postcodes op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat (postcode).</param>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de postcodes (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met postinfo over postcodes gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("postinfo.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(List<PostalInformationListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PostalInformationListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListPostalCodesWithFormat(
            [FromRoute] string format,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeentenaam,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<PostalOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = DetermineAndSetContentFormat(format, actionContextAccessor, Request);

            const Taal taal = Taal.NL;

            IRestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                gemeentenaam);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/postalinfo-list:{taal}");

            var value = await (CacheToggle.FeatureEnabled
                ? GetFromCacheThenFromBackendAsync(
                    format,
                    BackendRequest,
                    cacheKey,
                    Request.GetTypedHeaders(),
                    CreateDefaultHandleBadRequest(),
                    actionContextAccessor.ActionContext.ActionDescriptor,
                    cancellationToken)
                : GetFromBackendAsync(
                    format,
                    BackendRequest,
                    Request.GetTypedHeaders(),
                    CreateDefaultHandleBadRequest(),
                    actionContextAccessor.ActionContext.ActionDescriptor,
                    cancellationToken));

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            Taal language,
            string sort,
            string municipalityName)
        {
            var filter = new PostalInformationFilter
            {
                MunicipalityName = municipalityName
            };

            // postcode
            var sortMapping = new Dictionary<string, string>
            {
                { "PostCode", "PostalCode" },
            };

            return new RestRequest("postcodes?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
