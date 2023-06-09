namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class ConsumerStatusResponse : ListResponse<RegistryConsumerStatusResponse> { }

    public class RegistryConsumerStatusResponse
    {
        [DataMember(Order = 1)]
        public IEnumerable<RegistryConsumerStatus> Consumers { get; set; }
    }

    public class RegistryConsumerStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public DateTimeOffset DateProcessed { get; set; }
    }
}
