namespace Public.Api.BuildingUnit.Oslo
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using BuildingRegistry.Api.Oslo.BuildingUnit.List;
    using BuildingRegistry.Api.Oslo.BuildingUnit.Query;
    using Common.Infrastructure;
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

    public partial class BuildingUnitOsloController
    {
        /// <summary>
        /// Vraag een lijst met gebouweenheden op (v2).
        /// </summary>
        /// <param name="offset">Nulgebaseerde index van de eerste instantie die teruggegeven wordt (optioneel). De offset is echter beperkt tot 1000000, indien meer data dient ingelezen te worden is het gebruik van extra filters aangewezen op de service of verwijzen we naar de downloadproducten van GRAR.</param>
        /// <param name="limit">Aantal instanties dat teruggegeven wordt. Maximaal kunnen er 500 worden teruggegeven. Wanneer limit niet wordt meegegeven dan default 100 instanties (optioneel).</param>
        /// <param name="sort">Optionele sortering van het resultaat (id).</param>
        /// <param name="gebouwObjectId">Filter op de objectidentificator van het gekoppelde gebouw (exact) (optioneel).</param>
        /// <param name="adresObjectId">Filter op de objectidentificator van het gekoppelde adres (exact) (optioneel).</param>
        /// <param name="status">
        /// Filter op de status van de gebouweenheid (exact).<br/>
        /// `"gepland"` `"gerealiseerd"` `"gehistoreerd"` `"nietGerealiseerd"`
        /// </param>
        /// <param name="functie">Filter op de functie van een gebouweenheid (exact) (optioneel).</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gebouweenheden gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("gebouweenheden", Name = nameof(ListBuildingUnitsV2))]
        [ApiOrder(ApiOrder.BuildingUnit.V2 + 2)]
        [ProducesResponseType(typeof(BuildingUnitListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingUnitListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListBuildingUnitsV2(
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string sort,
            [FromQuery] int? gebouwObjectId,
            [FromQuery] int? adresObjectId,
            [FromQuery] string status,
            [FromQuery] string? functie,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<BuildingOptionsV2> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
            const Taal taal = Taal.NL;

            RestRequest BackendRequest() => CreateBackendListRequest(
                offset,
                limit,
                taal,
                gebouwObjectId,
                adresObjectId,
                functie,
                sort,
                status);

            var value = await GetFromBackendAsync(
                    contentFormat.ContentType,
                    BackendRequest,
                    CreateDefaultHandleBadRequest(),
                    cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.GebouweenheidVolgendeUrl);
        }

        private static RestRequest CreateBackendListRequest(
            int? offset,
            int? limit,
            Taal language,
            int? gebouwId,
            int? addressId,
            string? functie,
            string sort,
            string status)
        {
            var filter = new BuildingUnitFilter
            {
                BuildingPersistentLocalId = gebouwId,
                AddressPersistentLocalId = addressId?.ToString(),
                Status = status,
                Functie = functie
            };

            var sortMapping = new Dictionary<string, string>
            {
                { "Id", "PersistentLocalId" },
            };

            return new RestRequest("gebouweenheden?taal={language}")
                .AddParameter("language", language, ParameterType.UrlSegment)
                .AddPagination(offset, limit)
                .AddFiltering(filter)
                .AddSorting(sort, sortMapping);
        }
    }
}
