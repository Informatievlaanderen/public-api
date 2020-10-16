namespace Public.Api.Status.Responses
{
    using System.Runtime.Serialization;

    public class CacheStatusResponse : ListResponse<RegistryCacheStatus>
    {
    }

    public class RegistryCacheStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public long NumberOfRecordsToProcess { get; set; }
    }
}
