namespace Public.Api.CrabBuilding
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using BuildingRegistry.Api.Legacy.Building.Query;
    using BuildingRegistry.Api.Legacy.Building.Responses;
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

    public partial class CrabBuildingController
    {
        /// <summary>
        /// Vraag een lijst met CRAB gebouwen op die voldoen aan de filterparameters.
        /// </summary>
        /// <param name="terreinObjectId">Filter op de CRAB-TerreinObjectId van het gebouw (exact).</param>
        /// <param name="identificatorTerreinObject">
        /// Filter op het CRAB-IdentificatorTerreinObject van het gebouw (exact).<br/>
        /// (= OIDN van de corresponderende GRB-gebouwgeometrie)<br/>
        /// (= enige identificator waarmee in Lara op gebouw kan gezocht worden)
        /// </param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="responseOptions"></param>
        /// <param name="ifNoneMatch">If-None-Match header met ETag van een vorig verzoek (optioneel). </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met CRAB gebouwen gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("crabgebouwen", Name = nameof(ListCrabBuildings))]
        [ProducesResponseType(typeof(BuildingCrabMappingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingCrabMappingResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultListCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> ListCrabBuildings(
            [FromQuery] int? terreinObjectId,
            [FromQuery] string identificatorTerreinObject,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<AddressOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            if (!terreinObjectId.HasValue && string.IsNullOrEmpty(identificatorTerreinObject))
                throw new ApiException("Er dient minstens één identificator als input te worden meegegeven.", StatusCodes.Status400BadRequest);

            IRestRequest BackendRequest() => CreateBackendListRequest(
               terreinObjectId,
               identificatorTerreinObject);

            var cacheKey = CreateCacheKeyForRequestQuery($"legacy/crabgebouwen-list:{Taal.NL}");

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

            return BackendListResponseResult.Create(value, Request.Query, string.Empty);
        }

        private static IRestRequest CreateBackendListRequest(
            int? terrainObjectId,
            string identificatorTerrainObject)
        {
            var filter = new BuildingCrabMappingFilter
            {
                TerrainObjectId = terrainObjectId,
                IdentifierTerrainObject = identificatorTerrainObject
            };

            return new RestRequest("gebouwen/crabgebouwen")
                .AddFiltering(filter);
        }
    }
}
