namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class ProjectionStatusClient : BaseStatusClient<IEnumerable<RegistryProjectionStatus>, IEnumerable<ProjectionStatusClient.ProjectionStatus>>
    {
        public ProjectionStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient)
        { }

        protected override IRestRequest CreateStatusRequest()
            => new RestRequest("projections");

        protected override IEnumerable<RegistryProjectionStatus> Map(IEnumerable<ProjectionStatusClient.ProjectionStatus> response)
            => response.Select(status =>
                new RegistryProjectionStatus
                {
                    Name = status.Name,
                    State = status.State,
                });

        public class ProjectionStatus
        {
            public string Name { get; set; }
            public string State { get; set; }
        }
    }
}
