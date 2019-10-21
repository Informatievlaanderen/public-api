namespace Public.Api.Feeds
{
    using System.Net;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("feeds")]
    [ApiExplorerSettings(GroupName = "Feeds", IgnoreApi = true)]
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
