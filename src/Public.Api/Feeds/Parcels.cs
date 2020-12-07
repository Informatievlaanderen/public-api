namespace Public.Api.Feeds
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api.LastObservedPosition;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using ParcelRegistry.Api.Legacy.Parcel.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class FeedController
    {
        /// <summary>
        /// Vraag een lijst met wijzigingen op percelen op in het Atom formaat.
        /// </summary>
        /// <param name="actionContextAccessor"></param>
        /// <param name="restClients"></param>
        /// <param name="responseOptions"></param>
        /// <param name="from">Eventidentificator (volgnummer) vanaf waar de feed moet gestart of hernomen worden.</param>
        /// <param name="limit">Maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="embed">Keuze welke info in het <![CDATA[&lt;Content&gt;]]>-gedeelte van de output moet zitten. ("event", "object", "event/object")</param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met percelen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen", Name = nameof(GetParcelsFeed))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelSyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultFeedCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> GetParcelsFeed(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IIndex<string, Lazy<IRestClient>> restClients,
            [FromServices] IOptions<ParcelOptions> responseOptions,
            [FromQuery] long? from,
            [FromQuery] int? limit,
            [FromQuery] string embed,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await GetParcelsFeedWithFormat(
                null,
                actionContextAccessor,
                restClients,
                responseOptions,
                from,
                limit,
                embed,
                ifNoneMatch,
                cancellationToken);

        /// <summary>
        /// Vraag een lijst met wijzigingen op percelen op in het XML of Atom formaat.
        /// </summary>
        /// <param name="format">Gewenste formaat: percelen.xml of percelen.atom</param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="restClients"></param>
        /// <param name="responseOptions"></param>
        /// <param name="from">Eventidentificator (volgnummer) vanaf waar de feed moet gestart of hernomen worden.</param>
        /// <param name="limit">Maximaal aantal instanties dat teruggegeven wordt.</param>
        /// <param name="embed">Keuze welke info in het <![CDATA[&lt;Content&gt;]]>-gedeelte van de output moet zitten. ("event", "object", "event/object")</param>
        /// <param name="ifNoneMatch">Optionele If-None-Match header met ETag van een vorig verzoek.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met percelen gelukt is.</response>
        /// <response code="304">Als de lijst niet gewijzigd is ten opzicht van uw verzoek.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("percelen.{format}", Name = nameof(GetParcelsFeedWithFormat))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status304NotModified)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De ETag van de respons.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParcelSyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status304NotModified, typeof(NotModifiedResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(NotAcceptableResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = DefaultFeedCaching, NoStore = true, NoTransform = true)]
        public async Task<IActionResult> GetParcelsFeedWithFormat(
                 [FromRoute] string format,
                 [FromServices] IActionContextAccessor actionContextAccessor,
                 [FromServices] IIndex<string, Lazy<IRestClient>> restClients,
                 [FromServices] IOptions<ParcelOptions> responseOptions,
                 [FromQuery] long? from,
                 [FromQuery] int? limit,
                 [FromQuery] string embed,
                 [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
                 CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(format, actionContextAccessor.ActionContext);

            IRestRequest BackendRequest() => CreateBackendSyndicationRequest(
                "percelen",
                from,
                limit,
                embed);

            var value = await GetFromBackendAsync(
                restClients[RegistryKeys.Parcel].Value,
                BackendRequest,
                contentFormat.ContentType,
                HandleBadRequest,
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.Syndication.NextUri, contentFormat.UrlExtension);
        }
    }
}
