namespace Public.Api.StreetName.BackOffice
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;

    public partial class StreetNameBackOfficeController
    {
        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="streetNameProposeRequest"></param>
        /// <response code="201">Als de straatnaam voorgesteld is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string", "De url van de voorgestelde straatnaam.", "")]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "x-correlation-id", "string", "Correlatie identificator van de respons.")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerOperation(Description = "Voer een nieuwe straatnaam in met status  `\"voorgesteld\"`.")]
        [HttpPost("straatnamen/voorgesteld", Name = nameof(ProposeStreetName))]
        public async Task<IActionResult> ProposeStreetName([FromBody] StreetNameProposeRequest streetNameProposeRequest)
        {

            return Created(new Uri(string.Format("", "1")), null);
        }
    }
}
