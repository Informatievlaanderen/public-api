namespace Public.Api.Address.Oslo
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Oslo.Address.Search;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class AddressOsloController
    {
        private const int MaxSearchLimit = 50;
        private const int DefaultSearchLimit = 10;

        /// <summary>
        /// Zoeken naar adressen of straatnamen via een query.
        /// </summary>
        /// <param name="query">De zoek query.</param>
        /// <param name="municipalityOrPostalName">Limiteer de zoek query in een te zoeken gemeente- of postnaam (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 50 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 10 instanties (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van het adres of de straatnaam (exact) (optioneel). \
        /// `"voorgesteld"` `"inGebruik"` `"gehistoreerd"` `"afgekeurd"`
        /// </param>
        /// <param name="searchAddressesToggle"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van de zoekopdracht gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("adressen/zoeken", Name = nameof(SearchAddresses))]
        [ApiOrder(ApiOrder.Address.V2 + 5)]
        [ApiProduces(EndpointType.Json)]
        [ProducesResponseType(typeof(AddressSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressSearchResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> SearchAddresses(
            [FromQuery(Name = "q")] string? query,
            [FromQuery(Name="gemeenteOfPostNaam")] string? municipalityOrPostalName,
            [FromQuery] int? limit,
            [FromQuery] string? status,
            [FromServices] SearchAddressesToggle searchAddressesToggle,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptionsV2> responseOptions,
            CancellationToken cancellationToken = default)
        {
            if (!searchAddressesToggle.FeatureEnabled)
            {
                return NotFound();
            }

            limit = int.Max(0, int.Min(MaxSearchLimit, limit ?? DefaultSearchLimit));
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() => CreateBackendListRequest(
                limit,
                query,
                municipalityOrPostalName,
                status);

            var value = await  GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static RestRequest CreateBackendListRequest(
            int? limit,
            string? query,
            string? municipalityOrPostalName,
            string? status)
        {
            var filter = new AddressSearchFilter
            {
                Query = query,
                MunicipalityOrPostalName = municipalityOrPostalName,
                Status = status
            };

            return new RestRequest("adressen/zoeken")
                .AddPagination(0, limit)
                .AddFiltering(filter);
        }
    }
}
