namespace Public.Api.Feeds
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("feeds")]
    [ApiExplorerSettings(GroupName = "Feeds")]
    [Produces("application/atom+xml", "text/xml")]
    public partial class FeedController : ApiController<FeedController>
    {
        public FeedController(
            ConnectionMultiplexerProvider redis,
            ILogger<FeedController> logger)
            : base(redis, logger) { }

        private static IRestRequest CreateBackendSyndicationRequest(
            string resourcename,
            long? from,
            int? offset,
            int? limit,
            string embed)
            => new RestRequest($"{resourcename}/sync")
                    .AddPagination(offset, limit)
                    .AddFiltering(new
                    {
                        position = from ?? 0,
                        embed = embed ?? string.Empty
                    });
    }
}
