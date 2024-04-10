namespace Public.Api.Status.Clients
{
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class ConsumerStatusClient : BaseStatusClient<RegistryConsumerStatusResponse, ConsumerStatusList>
    {
        public ConsumerStatusClient(string registry, RestClient restClient)
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
