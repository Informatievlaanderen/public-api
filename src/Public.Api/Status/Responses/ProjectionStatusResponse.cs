namespace Public.Api.Status.Responses
{
    using System.Runtime.Serialization;

    public class ProjectionStatusResponse : ListResponse<RegistryProjectionStatus> { }

    public class RegistryProjectionStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string State { get; set; }
    }
}
