namespace Public.Api.CrabHouseNumber
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.CrabHouseNumber;
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

    public partial class CrabHouseNumberController
    {
        /// <summary>
        /// Vraag een lijst met CRAB huisnummers op.
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel).</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id).</param>
        /// <param name="objectId">Filter op de CRAB huisnummerid van het adres (exact) (optioneel).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met CRAB huisnummers gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("crabhuisnummers", Name = nameof(ListCrabHouseNumbers))]
        [ProducesResponseType(typeof(CrabHouseNumberAddressListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CrabHouseNumberListResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListCrabHouseNumbers(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] int? objectId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            IRestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                sort,
                objectId);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/crabhousenumbers-list:{Taal.NL}");

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

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.CrabHuisnummersVolgendeUrl);
        }

        private static IRestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            string sort,
            int? crabHouseNumberId)
        {
            var filter = new CrabHouseNumberAddressFilter
            {
                CrabHouseNumberId = crabHouseNumberId?.ToString()
            };

            var sortMapping = new Dictionary<string, string>
            {
                { "CrabHuisNummerId", "HouseNumberId" },
                { "CrabHuisNummer", "HouseNumberId" },
                { "HuisNummer", "HouseNumberId" },
                { "HuisNummerId", "HouseNumberId" },
                { "Id", "HouseNumberId" },
            };

            return new RestRequest("crabhuisnummers")
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
