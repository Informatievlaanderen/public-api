namespace Public.Api.Feeds
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Microsoft.Extensions.Logging;

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

        private static RestRequest CreateBackendSyndicationRequest(string resourcename, long from, int offset, int limit, bool embed)
        {
            var request = new RestRequest($"{resourcename}/sync?embed={embed}");
            request.AddHeader(AddPaginationExtension.HeaderName, $"{offset},{limit}");
            request.AddHeader(ExtractFilteringRequestExtension.HeaderName, $"{{ position: {from} }}");
            return request;
        }
    }
}
