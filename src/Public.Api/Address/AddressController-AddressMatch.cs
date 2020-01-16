namespace Public.Api.Address
{
    using AddressRegistry.Api.Legacy.AddressMatch.Responses;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using System.Threading;
    using System.Threading.Tasks;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressController
    {
        /// <summary>
        /// Voer een adres match vraag uit en krijg de adressen die gematcht worden.
        /// </summary>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de adressen.</param>
        /// <param name="niscode">Filter op de NisCode van de gemeente.</param>
        /// <param name="postcode">Filter op de postcode van het adres.</param>
        /// <param name="kadStraatcode">Filter op de straatcode van het kadaster.</param>
        /// <param name="rrStraatcode">Filter op de straatcode van het rijksregister.</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres.</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres.</param>
        /// <param name="index">Filter op het huisnummer gekend in het rijksregister.</param>
        /// <param name="busnummer">Filter op het busnummer van het adres.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adresmatch")]
        [ProducesResponseType(typeof(AddressMatchCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> AddressMatch(
            [FromQuery] string gemeentenaam,
            [FromQuery] int? niscode,
            [FromQuery] int? postcode,
            [FromQuery] int? kadStraatcode,
            [FromQuery] string rrStraatcode,
            [FromQuery] string straatnaam,
            [FromQuery] string huisnummer,
            [FromQuery] string index,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            CancellationToken cancellationToken = default)
            => await AddressMatchWithFormat(
                null,
                gemeentenaam,
                niscode,
                postcode,
                kadStraatcode,
                rrStraatcode,
                straatnaam,
                huisnummer,
                index,
                busnummer,
                actionContextAccessor,
                responseOptions,
                cancellationToken);

        /// <summary>
        /// Voer een adres match vraag uit en krijg de adressen die gematcht worden.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de adressen.</param>
        /// <param name="niscode">Filter op de NisCode van de gemeente.</param>
        /// <param name="postcode">Filter op de postcode van het adres.</param>
        /// <param name="kadStraatcode">Filter op de straatcode van het kadaster.</param>
        /// <param name="rrStraatcode">Filter op de straatcode van het rijksregister.</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres.</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres.</param>
        /// <param name="index">Filter op het huisnummer gekend in het rijksregister.</param>
        /// <param name="busnummer">Filter op het busnummer van het adres.</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met adressen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adresmatch.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(AddressMatchCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> AddressMatchWithFormat(
            [FromRoute] string format,
            [FromQuery] string gemeentenaam,
            [FromQuery] int? niscode,
            [FromQuery] int? postcode,
            [FromQuery] int? kadStraatcode,
            [FromQuery] string rrStraatcode,
            [FromQuery] string straatnaam,
            [FromQuery] string huisnummer,
            [FromQuery] string index,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            var taal = Taal.NL;

            IRestRequest BackendRequest() => CreateBackendMatchRequest(
                taal,
                busnummer,
                huisnummer,
                postcode,
                gemeentenaam,
                niscode,
                straatnaam,
                kadStraatcode,
                rrStraatcode,
                index);

            var response = await GetFromBackendWithBadRequestAsync(format, BackendRequest, Request.GetTypedHeaders(), CreateDefaultHandleBadRequest(), cancellationToken);

            return BackendListResponseResult.Create(response, Request.Query, string.Empty);
        }

        private static IRestRequest CreateBackendMatchRequest(
            Taal language,
            string boxNumber,
            string houseNumber,
            int? postalCode,
            string municipalityName,
            int? nisCode,
            string streetName,
            int? kadStreetCode,
            string rrStreetCode,
            string index)
        {
            var request = new RestRequest("adresmatch")
                .AddParameter("taal", language, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(boxNumber))
                request.AddParameter("busnummer", boxNumber, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(houseNumber))
                request.AddParameter("huisnummer", houseNumber, ParameterType.QueryString);

            if (postalCode.HasValue)
                request.AddParameter("postcode", postalCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(municipalityName))
                request.AddParameter("gemeentenaam", municipalityName, ParameterType.QueryString);

            if (nisCode.HasValue)
                request.AddParameter("niscode", nisCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(streetName))
                request.AddParameter("straatnaam", streetName, ParameterType.QueryString);

            if (kadStreetCode.HasValue)
                request.AddParameter("kadstraatcode", kadStreetCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(rrStreetCode))
                request.AddParameter("rrstraatcode", rrStreetCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(index))
                request.AddParameter("index", index, ParameterType.QueryString);

            return request;
        }
    }
}
