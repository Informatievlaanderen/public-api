namespace Public.Api.Address
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.Address.Query;
    using AddressRegistry.Api.Legacy.Address.Responses;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.LastObservedPosition;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressController
    {
        /// <summary>
        /// Vraag het totaal aantal adressen op.
        /// </summary>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de adressen (exact).</param>
        /// <param name="postcode">Filter op de postcode van het adres (exact).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres (exact).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniem toevoeging van het adres (exact).</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres (exact).</param>
        /// <param name="busnummer">Filter op het busnummer van het adres (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal adressen gelukt is.</response>
        /// <response code="304">Als het aantal niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen/totaal-aantal", Name = nameof(CountAddresses))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> CountAddresses(
            [FromQuery] string gemeentenaam,
            [FromQuery] int? postcode,
            [FromQuery] string straatnaam,
            [FromQuery] string homoniemToevoeging,
            [FromQuery] string huisnummer,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await CountAddressesWithFormat(
                null,
                gemeentenaam,
                postcode,
                straatnaam,
                homoniemToevoeging,
                huisnummer,
                busnummer,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag het totaal aantal adressen op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de adressen (exact).</param>
        /// <param name="postcode">Filter op de postcode van het adres (exact).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres (exact).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniem toevoeging van het adres (exact).</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres (exact).</param>
        /// <param name="busnummer">Filter op het busnummer van het adres (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal adressen gelukt is.</response>
        /// <response code="304">Als het aantal niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen/totaal-aantal.{format}", Name = nameof(CountAddressesWithFormat))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> CountAddressesWithFormat(
            [FromRoute] string format,
            [FromQuery] string gemeentenaam,
            [FromQuery] int? postcode,
            [FromQuery] string straatnaam,
            [FromQuery] string homoniemToevoeging,
            [FromQuery] string huisnummer,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(format, actionContextAccessor.ActionContext);

            IRestRequest BackendRequest() => CreateBackendCountRequest(
                busnummer,
                huisnummer,
                postcode,
                gemeentenaam,
                straatnaam,
                homoniemToevoeging);

            return new BackendResponseResult(
                await GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken));
        }

        private static IRestRequest CreateBackendCountRequest(
            string boxNumber,
            string houseNumber,
            int? postalCode,
            string municipalityName,
            string streetName,
            string homonymAddition)
        {
            var filter = new AddressFilter
            {
                BoxNumber = boxNumber,
                HouseNumber = houseNumber,
                PostalCode = postalCode?.ToString(),
                MunicipalityName = municipalityName,
                StreetName = streetName,
                HomonymAddition = homonymAddition
            };

            return new RestRequest("adressen/totaal-aantal")
                .AddFiltering(filter);
        }
    }
}
