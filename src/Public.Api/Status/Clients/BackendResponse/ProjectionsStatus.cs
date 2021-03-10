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
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public long CurrentPosition { get; set; }
        public string ErrorMessage { get; set; }
    }
}
