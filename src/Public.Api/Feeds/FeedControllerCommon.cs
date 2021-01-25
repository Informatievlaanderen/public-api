namespace Public.Api.Feeds
{
    using System.Net;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("feeds")]
    [ApiExplorerSettings(GroupName = FeedsGroupName)]
    [ApiOrder(Order = ApiOrder.Feeds)]
    [ApiProduces(EndpointType.Sync)]
    [ApiKeyAuth("Sync")]
    public partial class FeedController : ApiController<FeedController>
    {
        public const string FeedsGroupName = "Feeds";

        protected const int DefaultFeedCaching = 0;
        private const int NoPaging = 0;

        public FeedController(
            ConnectionMultiplexerProvider redis,
            ILogger<FeedController> logger)
            : base(redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Sync, context);

        private static IRestRequest CreateBackendSyndicationRequest(
            string resourcename,
            long? from,
            int? limit,
            EmbedValue embed)
            => new RestRequest($"{resourcename}/sync")
                    .AddPagination(NoPaging, limit)
                    .AddFiltering(new
                    {
                        position = from ?? 0,
                        embed = embed.ToString()
                    });

        protected void HandleBadRequest(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotAcceptable:
                    throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);

                case HttpStatusCode.BadRequest:
                    throw new ApiException("Ongeldige vraag.", StatusCodes.Status400BadRequest);
            }
        }
    }
}
