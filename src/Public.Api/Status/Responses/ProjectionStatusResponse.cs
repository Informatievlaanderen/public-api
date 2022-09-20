namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class ProjectionStatusResponse : ListResponse<RegistryProjectionStatusResponse>
    {
        public ProjectionStatusResponse()
        { }
        
        private ProjectionStatusResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }

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
        public string Key { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public string State { get; set; }

        [DataMember(Order = 5)]
        public long CurrentPosition { get; set; }
    }
}
