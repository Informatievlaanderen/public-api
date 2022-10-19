namespace Public.Api.Address.Oslo
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Oslo.Address.Query;
    using AddressRegistry.Api.Oslo.Address.Responses;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressOsloController
    {
        /// <summary>
        /// Vraag het totaal aantal adressen op (v2).
        /// </summary>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van het adres (exact) (optioneel).</param>
        /// <param name="postcode">Filter op de postcode van het adres (exact) (optioneel).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres (exact) (optioneel).</param>
        /// <param name="homoniemToevoeging">Filter op de homoniemtoevoeging van het adres (exact) (optioneel).</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres (exact) (optioneel).</param>
        /// <param name="busnummer">Filter op het busnummer van het adres (exact) (optioneel).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen/totaal-aantal", Name = nameof(CountAddressesV2))]
        [ApiOrder(ApiOrder.Address.V2 + 3)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountOsloResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> CountAddressesV2(
            [FromQuery] string gemeentenaam,
            [FromQuery] int? postcode,
            [FromQuery] string straatnaam,
            [FromQuery] string homoniemToevoeging,
            [FromQuery] string huisnummer,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromServices] IsAddressOsloApiEnabledToggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
                return NotFound();

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendCountRequest(
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

        private static RestRequest CreateBackendCountRequest(
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
