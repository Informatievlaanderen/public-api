namespace Public.Api.Status.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class BackOfficeStatusResponse : ListResponse<RegistryBackOfficeStatusResponse> { }

    public class RegistryBackOfficeStatusResponse
    {
        [DataMember(Order = 1)]
        public IEnumerable<RegistryBackOfficeStatus> projections { get; set; }
    }

    public class RegistryBackOfficeStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public long CurrentPosition { get; set; }

        [DataMember(Order = 3)]
        public long MaxPosition { get; set; }
    }
}
