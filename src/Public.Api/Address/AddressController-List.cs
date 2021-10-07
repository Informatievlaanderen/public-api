namespace Public.Api.Address
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.Address.Query;
    using AddressRegistry.Api.Legacy.Address.Responses;
    using Be.Vlaanderen.Basisregisters.Api.LastObservedPosition;
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
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressController
    {
        /// <summary>
        /// Vraag een lijst met adressen op.
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id, postcode, huisnummer, busnummer).</param>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van het adres (exact) (optioneel).</param>
        /// <param name="postcode">Filter op de postcode van het adres (exact) (optioneel).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres (exact) (optioneel).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniemtoevoeging van het adres (exact) (optioneel).</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres (exact) (optioneel).</param>
        /// <param name="busnummer">Filter op het busnummer van het adres (exact) (optioneel).</param>
        /// <param name="nisCode">Filter op de niscode van het adres (exact) (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van het adres (exact) (optioneel).<br/>
        /// `"voorgesteld"` `"inGebruik"` `"gehistoreerd"`
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen", Name = nameof(ListAddresses))]
        [ProducesResponseType(typeof(AddressListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressListResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListAddresses(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] string gemeentenaam,
            [FromQuery] int? postcode,
            [FromQuery] string straatnaam,
            [FromQuery] string homoniemToevoeging,
            [FromQuery] string huisnummer,
            [FromQuery] string busnummer,
            [FromQuery] string nisCode,
            [FromQuery] string status,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            const Taal taal = Taal.NL;

            IRestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                sort,
                busnummer,
                huisnummer,
                postcode,
                gemeentenaam,
                straatnaam,
                homoniemToevoeging,
                nisCode,
                status);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/address-list:{taal}");

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
            string boxNumber,
            string houseNumber,
            int? postalCode,
            string municipalityName,
            string streetName,
            string homonymAddition,
            string nisCode,
            string status)
        {
            var filter = new AddressFilter
            {
                BoxNumber = boxNumber,
                HouseNumber = houseNumber,
                PostalCode = postalCode?.ToString(),
                MunicipalityName = municipalityName,
                StreetName = streetName,
                HomonymAddition = homonymAddition,
                NisCode = nisCode,
                Status = status
            };

            // id, postcode, huisnummer, busnummer
            var sortMapping = new Dictionary<string, string>
            {
                { "BusNummer", "BoxNumber" },
                { "huisnummer", "HouseNumber" },
                { "postcode", "PostalCode" },
                { "Id", "PersistentLocalId" },
            };

            return new RestRequest("adressen?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
