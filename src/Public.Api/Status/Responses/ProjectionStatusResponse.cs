namespace Public.Api.Status.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class ProjectionStatusResponse : ListResponse<RegistryProjectionStatusResponse> { }

    public class RegistryProjectionStatusResponse
    {
        [DataMember(Order = 1)]
        public long StreamPosition { get; set; }

        [DataMember(Order = 2)]
        public IEnumerable<RegistryProjectionStatus> Projections { get; set; }
    }

    public class RegistryProjectionStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string State { get; set; }

        [DataMember(Order = 3)]
        public long CurrentPosition { get; set; }
    }
}
