namespace Public.Api.Status.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class ProjectionStatusClient : BaseStatusClient<RegistryProjectionStatusResponse, ProjectionsStatusList>
    {
        private readonly IEnumerable<ProjectionInfo.Info> _registryInfo;

        public ProjectionStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient)
        {
            _registryInfo = new ProjectionInfo().For(registry);
        }

        protected override IRestRequest CreateStatusRequest()
            => new RestRequest("projections");

        protected override RegistryProjectionStatusResponse Map(ProjectionsStatusList response)
            => new RegistryProjectionStatusResponse
            {
                StreamPosition = response.StreamPosition,
                Projections = response
                    .Projections
                    .Select(CreateStatus)
            };

        private RegistryProjectionStatus CreateStatus(ProjectionStatus status)
        {
            var info = _registryInfo.SingleOrDefault(projectionInfo => projectionInfo.Key.Equals(status.ProjectionName, StringComparison.InvariantCultureIgnoreCase));
            return new RegistryProjectionStatus
            {
                Key = status.ProjectionName,
                Name = string.IsNullOrWhiteSpace(info?.Name) ? status.ProjectionName : info.Name,
                Description = info?.Description ?? string.Empty,
                State = status.ProjectionState,
                CurrentPosition = status.CurrentPosition
            };
        }
    }
}
