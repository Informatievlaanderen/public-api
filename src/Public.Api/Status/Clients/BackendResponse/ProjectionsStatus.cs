namespace Public.Api.Status.Clients.BackendResponse
{
    using System.Collections.Generic;

    public class ProjectionsStatusList
    {
        public List<ProjectionStatus> Projections { get; set; }
        public long StreamPosition { get; set; }
    }

    public class ProjectionStatus
    {
        public string ProjectionName { get; set; }
        public string ProjectionState { get; set; }
        public long CurrentPosition { get; set; }
        public string ErrorMessage { get; set; }
    }
}
