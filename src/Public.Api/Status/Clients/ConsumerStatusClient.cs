namespace Public.Api.Status.Clients
{
    using System.Linq;
    using BackendResponse;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class ConsumerStatusClient : BaseStatusClient<RegistryConsumerStatusResponse, ConsumerStatusList>
    {
        public ConsumerStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("consumers");

        protected override RegistryConsumerStatusResponse Map(ConsumerStatusList response)
            => new RegistryConsumerStatusResponse
            {
                Consumers = response
                    .Select(status =>
                        new RegistryConsumerStatus
                        {
                            Name = status.Name,
                            DateProcessed = status.LastProcessedMessage
                        })
            };
    }
}
