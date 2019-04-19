namespace Public.Api.Address
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.Address.Query;
    using AddressRegistry.Api.Legacy.Address.Responses;
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
    using Swashbuckle.AspNetCore.Filters;

    public partial class AddressController
    {
        /// <summary>
        /// Vraag een lijst met actieve adressen op.
        /// </summary>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, postcode, huisnummer, busnummer).</param>
        /// <param name="busNummer">Filter op het busnummer van het adres (exact).</param>
        /// <param name="huisNummer">Filter op het huisnummer van het adres (exact).</param>
        /// <param name="postCode">Filter op de postcode van het adres (exact).</param>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de adressen (exact).</param>
        /// <param name="straatNaam">Filter op de straatnaam van het adres (exact).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniem toevoeging van het adres (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen")]
        [ProducesResponseType(typeof(List<AddressListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> ListAddresses(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string busNummer,
            [FromQuery] string huisNummer,
            [FromQuery] string postCode,
            [FromQuery] string gemeenteNaam,
            [FromQuery] string straatNaam,
            [FromQuery] string homoniemToevoeging,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await ListAddressesWithFormat(
                null,
                offset,
                limit,
                sort,
                busNummer,
                huisNummer,
                postCode,
                gemeenteNaam,
                straatNaam,
                homoniemToevoeging,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een lijst met actieve adressen op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="offset">Optionele nulgebaseerde index van de eerste instantie die teruggegeven wordt.</param>
        /// <param name="limit">Optioneel maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, postcode, huisnummer, busnummer).</param>
        /// <param name="busNummer">Filter op het busnummer van het adres (exact).</param>
        /// <param name="huisNummer">Filter op het huisnummer van het adres (exact).</param>
        /// <param name="postCode">Filter op de postcode van het adres (exact).</param>
        /// <param name="gemeenteNaam">De gerelateerde gemeentenaam van de adressen (exact).</param>
        /// <param name="straatNaam">Filter op de straatnaam van het adres (exact).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniem toevoeging van het adres (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen.{format}")]
        [ProducesResponseType(typeof(List<AddressListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressListResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheExpiration(MaxAge = 12 * 60 * 60)] // Hours, Minutes, Second
        public async Task<IActionResult> ListAddressesWithFormat(
            [FromRoute] string format,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string busNummer,
            [FromQuery] string huisNummer,
            [FromQuery] string postCode,
            [FromQuery] string gemeenteNaam,
            [FromQuery] string straatNaam,
            [FromQuery] string homoniemToevoeging,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
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
                busNummer,
                huisNummer,
                postCode,
                gemeenteNaam,
                straatNaam,
                homoniemToevoeging);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/address-list:{taal}");

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
            string boxNumber,
            string houseNumber,
            string postalCode,
            string municipalityName,
            string streetName,
            string homonymAddition)
        {
            var filter = new AddressFilter
            {
                BoxNumber = boxNumber,
                HouseNumber = houseNumber,
                PostalCode = postalCode,
                MunicipalityName = municipalityName,
                StreetName = streetName,
                HomonymAddition = homonymAddition
            };

            // id, postcode, huisnummer, busnummer
            var sortMapping = new Dictionary<string, string>
            {
                { "BusNummer", "BoxNumber" },
                { "HuisNummer", "HouseNumber" },
                { "PostCode", "PostalCode" },
                { "Id", "OsloId" },
            };

            return new RestRequest("adressen?taal={taal}")
                .AddParameter("taal", taal, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
