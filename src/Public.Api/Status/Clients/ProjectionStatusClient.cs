namespace Public.Api.Status.Clients
{
    using System.Linq;
    using BackendResponse;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class ProjectionStatusClient : BaseStatusClient<RegistryProjectionStatusResponse, ProjectionsStatusList>
    {
        public ProjectionStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient) { }

        protected override IRestRequest CreateStatusRequest()
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
                            Name = status.ProjectionName,
                            State = status.ProjectionState,
                            CurrentPosition = status.CurrentPosition
                        })
            };
    }
}
