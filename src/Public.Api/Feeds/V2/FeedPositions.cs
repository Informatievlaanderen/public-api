namespace Public.Api.Feeds.V2
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Autofac.Features.Indexed;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;
    using Version = Infrastructure.Version.Version;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [ApiRoute("feeds/posities")]
    [ApiExplorerSettings(GroupName = FeedV2Controller.FeedsGroupName)]
    [ApiOrder(ApiOrder.Feeds + 20)]
    [ApiProduces(EndpointType.Json)]
    [ApiKeyAuth("Sync")]
    public class FeedPositionsController : ApiController<FeedPositionsController>
    {
        public FeedPositionsController(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<FeedPositionsController> logger)
            : base(httpContextAccessor, redis, logger)
        { }

        /// <summary>
        /// Vraag de posities op van de feeds voor een bepaald register, aan de hand van een andere positie identificator.
        /// </summary>
        /// <param name="restClients"></param>
        /// <param name="register">Het register waarvoor je de posities opvraagt.</param>
        /// <param name="feed">Eventidentificator van de XML/Atom feeds (1) (optioneel).</param>
        /// <param name="download">De identificator in het dowloadbestand (_metadata.dbf) (1) (optioneel).</param>
        /// <param name="wijzigingFeedId">De Id van de entry in de wijzigingen feed (1) (optioneel).</param>
        /// <param name="feedPositionsToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Als de opvraging van de posities gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als er geen API key is meegegeven.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("", Name = nameof(FeedPositions))]
        [ProducesResponseType(typeof(FeedPositieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FeedPositieResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status429TooManyRequests, typeof(TooManyRequestsResponseExamplesV2))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamplesV2))]
        [HttpCacheValidation(NoCache = true, MustRevalidate = true, ProxyRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 0, NoStore = true, NoTransform = true)]
        [SwaggerOperation(Description = "Van de optionele parameters (1) moet er één ingevuld zijn.")]
        public async Task<IActionResult> FeedPositions(
            [FromServices] IIndex<string, Lazy<RestClient>> restClients,
            [FromQuery] FeedPositiesRegister register,
            [FromQuery] long? feed,
            [FromQuery] long? download,
            [FromQuery] long? wijzigingFeedId,
            [FromServices] FeedPositionsToggle feedPositionsToggle,
            CancellationToken cancellationToken = default)
        {
            if (!feedPositionsToggle.FeatureEnabled)
                return NotFound();

            RestRequest BackendRequest() => CreateRequest(register, feed, download, wijzigingFeedId);
            var value = await GetFromBackendAsync(
                restClients[RegistryKeysByEndpoint[register]].Value,
                BackendRequest,
                AcceptType.Json,
                HandleBadRequest,
                cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForRead());
        }

        private void HandleBadRequest(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotAcceptable:
                    throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);

                case HttpStatusCode.BadRequest:
                    throw new ApiException("Ongeldige vraag.", StatusCodes.Status400BadRequest);
            }
        }

        private static readonly Dictionary<FeedPositiesRegister, string> RegistryKeysByEndpoint = new()
        {
            { FeedPositiesRegister.Gemeente, RegistryKeys.MunicipalityV2 },
            { FeedPositiesRegister.Postinfo, RegistryKeys.PostalV2 },
            { FeedPositiesRegister.Straatnaam, RegistryKeys.StreetNameV2 },
            { FeedPositiesRegister.Adres, RegistryKeys.AddressV2 },
            { FeedPositiesRegister.Gebouw, RegistryKeys.BuildingV2 },
            { FeedPositiesRegister.Gebouweenheid, RegistryKeys.BuildingV2 },
            { FeedPositiesRegister.Perceel, RegistryKeys.ParcelV2 }
        };

        private static readonly Dictionary<FeedPositiesRegister, string> ResourceNames = new()
        {
            { FeedPositiesRegister.Gemeente, "gemeenten" },
            { FeedPositiesRegister.Postinfo, "postinfo" },
            { FeedPositiesRegister.Straatnaam, "straatnamen" },
            { FeedPositiesRegister.Adres, "adressen" },
            { FeedPositiesRegister.Gebouw, "gebouwen" },
            { FeedPositiesRegister.Gebouweenheid, "gebouweenheden" },
            { FeedPositiesRegister.Perceel, "percelen" }
        };

        private static RestRequest CreateRequest(
            FeedPositiesRegister register,
            long? feed,
            long? download,
            long? wijzigingFeedId)
            => new RestRequest($"{ResourceNames[register]}/posities")
                .AddFiltering(new
                {
                    Download = download,
                    Sync = feed,
                    ChangeFeedId = wijzigingFeedId,
                });

        [DataContract(Name = "FeedPositieRegister", Namespace = "")]
        public enum FeedPositiesRegister
        {
            Gemeente,
            Postinfo,
            Straatnaam,
            Adres,
            Gebouw,
            Gebouweenheid,
            Perceel
        }

        public sealed class FeedPositieResponseExample : IExamplesProvider<FeedPositieResponse>
        {
            public FeedPositieResponse GetExamples()
            {
                return new FeedPositieResponse
                {
                    Feed = 1000,
                    WijzigingenFeedId = 999,
                    WijzigingenFeedPagina = 5
                };
            }
        }
    }
}
