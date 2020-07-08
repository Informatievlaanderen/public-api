namespace Public.Api.StreetName
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
    using RestSharp;
    using StreetNameRegistry.Api.Legacy.StreetName.Query;
    using StreetNameRegistry.Api.Legacy.StreetName.Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameController
    {
        /// <summary>
        /// Vraag het totaal aantal straatnamen op.
        /// </summary>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de straatnamen (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal straatnamen gelukt is.</response>
        /// <response code="304">Als het aantal niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen/totaal-aantal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> CountStreetNames(
            [FromQuery] string gemeentenaam,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await CountStreetNamesWithFormat(
                null,
                gemeentenaam,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag het totaal aantal straatnamen op.
        /// </summary>
        /// <param name="format">Gewenste formaat: json of xml.</param>
        /// <param name="gemeentenaam">De gerelateerde gemeentenaam van de straatnamen (exact).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal straatnamen gelukt is.</response>
        /// <response code="304">Als het aantal niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("straatnamen/totaal-aantal.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultCountCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> CountStreetNamesWithFormat(
            [FromRoute] string format,
            [FromQuery] string gemeentenaam,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = DetermineAndSetContentFormat(format, actionContextAccessor, Request);

            IRestRequest BackendRequest() => CreateBackendCountRequest(gemeentenaam);

            return new BackendResponseResult(
                await GetFromBackendAsync(
                    format,
                    BackendRequest,
                    Request.GetTypedHeaders(),
                    CreateDefaultHandleBadRequest(),
                    actionContextAccessor.ActionContext.ActionDescriptor,
                    cancellationToken));
        }

        private static IRestRequest CreateBackendCountRequest(string municipalityName)
        {
            var filter = new StreetNameFilter
            {
                MunicipalityName = municipalityName
            };

            return new RestRequest("straatnamen/totaal-aantal")
                .AddFiltering(filter);
        }
    }
}
