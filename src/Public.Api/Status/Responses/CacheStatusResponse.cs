namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class CacheStatusResponse : ListResponse<IEnumerable<RegistryCacheStatus>>
    {
        public CacheStatusResponse()
        { }
    
        private CacheStatusResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }

    public class RegistryCacheStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public long NumberOfRecordsToProcess { get; set; }
    }
}
