namespace Public.Api.Feeds.V2.Change;

using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Common.Infrastructure;
using FeatureToggle;
using Infrastructure;
using Infrastructure.Configuration;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MunicipalityRegistry.Api.Oslo.Municipality.Responses;
using RestSharp;
using Swashbuckle.AspNetCore.Filters;

public partial class ChangeFeedV2Controller
{
    /// <summary>
    /// Vraag een lijst met wijzigingen op gemeenten in CloudEvents formaat (v2).
    /// </summary>
    /// <param name="actionContextAccessor"></param>
    /// <param name="restClients"></param>
    /// <param name="pagina">Paginanummer vanaf waar de feed moet gestart of hernomen worden (optioneel).</param>
    /// <param name="feedPositie">Positie van de XML/Atom feed die vertaalt wordt naar een pagina (optioneel). Let op: de pagina kunnen items bevatten die voor de gegeven positie liggen.</param>
    /// <param name="cacheToggle"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="ifNoneMatch"></param>
    /// <returns></returns>
    /// <response code="200">Als de opvraging van een lijst met wijzigingen op gemeenten gelukt is.</response>
    /// <response code="400">Als uw verzoek foutieve data bevat.</response>
    /// <response code="401">Als er geen API key is meegegeven.</response>
    /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
    /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
    /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("gemeenten", Name = nameof(ChangeFeedMunicipality))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de response.")]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "x-correlation-id", "string", "Correlatie identificator van de response.")]
    //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalitySyndicationResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamplesV2))]
    [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
    [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
    [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultFeedCaching, NoStore = true, NoTransform = true)]
    public async Task<IActionResult> ChangeFeedMunicipality(
        [FromServices] IActionContextAccessor actionContextAccessor,
        [FromServices] IIndex<string, Lazy<RestClient>> restClients,
        [FromQuery] int? pagina,
        [FromQuery] int? feedPositie,
        [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
        [KeyFilter(RegistryKeys.MunicipalityV2)] IFeatureToggle cacheToggle,
        CancellationToken cancellationToken = default)
    {
        var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);
        var cacheKey = $"feed/municipality:{pagina}";

        RestRequest BackendRequest() => CreateBackendChangeFeedRequest(
            "gemeenten",
            pagina,
            feedPositie);

        var value = await (CanGetFromCache(cacheToggle, actionContextAccessor.ActionContext)
            ? GetFromCacheThenFromBackendAsync(
                contentFormat.ContentType,
                restClients[RegistryKeys.MunicipalityV2].Value,
                BackendRequest,
                cacheKey,
                HandleBadRequest,
                cancellationToken)
            : GetFromBackendAsync(
                restClients[RegistryKeys.MunicipalityV2].Value,
                BackendRequest,
                contentFormat.ContentType,
                HandleBadRequest,
                cancellationToken));

        return new BackendResponseResult(value);
    }
}
