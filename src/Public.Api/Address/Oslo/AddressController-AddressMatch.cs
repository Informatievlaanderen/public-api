namespace Public.Api.Address.Oslo
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Oslo.AddressMatch.Responses;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressOsloController
    {
        /// <summary>
        /// Voer een adresmatch vraag uit en krijg de adressen die gematcht worden (v2).
        /// </summary>
        /// <param name="gemeentenaam">Filter op de gemeentenaam van het adres (1).</param>
        /// <param name="niscode">Filter op de NIS-code van het adres (1).</param>
        /// <param name="postcode">Filter op de postcode van het adres (1).</param>
        /// <param name="straatnaam">Filter op de straatnaam van het adres.</param>
        /// <param name="huisnummer">Filter op het huisnummer van het adres.</param>
        /// <param name="busnummer">Filter op het busnummer van het adres.</param>
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
        [HttpGet("adresmatch", Name = nameof(AddressMatchV2))]
        [ApiOrder(ApiOrder.Address.V2 + 4)]
        [ApiProduces(EndpointType.Oslo)]
        [ProducesResponseType(typeof(AddressMatchOsloCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressMatchResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [SwaggerOperation(Description = "Van de optionele parameters (1) moet er minstens één ingevuld zijn.")]
        public async Task<IActionResult> AddressMatchV2(
            [FromQuery] string gemeentenaam,
            [FromQuery] string niscode,
            [FromQuery] string postcode,
            [FromQuery] [Required] string straatnaam,
            [FromQuery] string huisnummer,
            [FromQuery] string busnummer,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
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
                straatnaam);

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
            string streetName)
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

            return request;
        }
    }
}
