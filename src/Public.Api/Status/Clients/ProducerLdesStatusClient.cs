namespace Public.Api.Status.Clients
{
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class ProducerLdesStatusClient : BaseStatusClient<RegistryProjectionStatusResponse, ProjectionsStatusList>
    {
        public ProducerLdesStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("projections");

        protected override RegistryProjectionStatusResponse Map(ProjectionsStatusList response)
            => new RegistryProjectionStatusResponse
            {
                StreamPosition = response.StreamPosition,
                Projections = response
                    .Projections
                    .Select(status =>
                        new RegistryProjectionStatus
                        {
                            Key = status.Id,
                            Name = string.IsNullOrWhiteSpace(status.Name) ? status.Id : status.Name,
                            Description = status.Description,
                            State = status.State,
                            CurrentPosition = status.CurrentPosition
                        })
            };
    }
}
