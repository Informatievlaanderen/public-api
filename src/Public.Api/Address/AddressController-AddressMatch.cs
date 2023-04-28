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
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Infrastructure.Swagger;
    using Swashbuckle.AspNetCore.Annotations;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressController
    {
        /// <summary>
        /// Voer een adresmatch vraag uit en krijg de adressen die gematcht worden (v1).
        /// </summary>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van het adres (1).</param>
        /// <param name="niscode">Filter op de NIS-code van het adres (1).</param>
        /// <param name="postcode">Filter op de postcode van het adres (1).</param>
        /// <param name="kadStraatcode">Filter op de straatcode van het kadaster (2) (3).</param>
        /// <param name="rrStraatcode">Filter op de straatcode van het rijksregister (2) (3).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres (2).</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres (1).</param>
        /// <param name="index">Filter op het huisnummer gekend in het rijksregister (4).</param>
        /// <param name="busnummer">Filter op het busnummer van het adres (4).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de adresmatch gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adresmatch", Name = nameof(AddressMatch))]
        [ApiOrder(ApiOrder.Address.V1 + 4)]
        [ProducesResponseType(typeof(AddressMatchCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Van de optionele parameters (2) resp. (1) moet er minstens één ingevuld zijn. <br />Van de optionele parameters (3) resp. (4) mag er maximaal één van de twee ingevuld zijn.")]
        public async Task<IActionResult> AddressMatch(
            [FromQuery] string gemeentenaam,
            [FromQuery] string niscode,
            [FromQuery] string postcode,
            [FromQuery] string kadStraatcode,
            [FromQuery] string rrStraatcode,
            [FromQuery] string straatnaam,
            [FromQuery] string huisnummer,
            [FromQuery] string index,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendMatchRequest(
                Taal.NL,
                busnummer,
                huisnummer,
                postcode,
                gemeentenaam,
                niscode,
                straatnaam,
                kadStraatcode,
                rrStraatcode,
                index);

            var response = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return BackendListResponseResult.Create(response, Request.Query, string.Empty);
        }

        private static RestRequest CreateBackendMatchRequest(
            Taal language,
            string boxNumber,
            string houseNumber,
            string postalCode,
            string municipalityName,
            string nisCode,
            string streetName,
            string kadStreetCode,
            string rrStreetCode,
            string index)
        {
            var request = new RestRequest("adresmatch")
                .AddParameter("taal", language, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(boxNumber))
                request.AddParameter("busnummer", boxNumber, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(houseNumber))
                request.AddParameter("huisnummer", houseNumber, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(postalCode))
                request.AddParameter("postcode", postalCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(municipalityName))
                request.AddParameter("gemeentenaam", municipalityName, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(nisCode))
                request.AddParameter("niscode", nisCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(streetName))
                request.AddParameter("straatnaam", streetName, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(kadStreetCode))
                request.AddParameter("kadstraatcode", kadStreetCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(rrStreetCode))
                request.AddParameter("rrstraatcode", rrStreetCode, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(index))
                request.AddParameter("index", index, ParameterType.QueryString);

            return request;
        }
    }
}
