namespace Public.Api.Status.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class SyndicationStatusResponse : ListResponse<RegistrySyndicationStatusResponse> { }

    public class RegistrySyndicationStatusResponse
    {
        [DataMember(Order = 1)]
        public IEnumerable<RegistrySyndicationStatus> Syndications { get; set; }
    }

    public class RegistrySyndicationStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public long CurrentPosition { get; set; }
    }
}
