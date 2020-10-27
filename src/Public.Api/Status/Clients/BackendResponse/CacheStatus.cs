namespace Public.Api.Status.Clients.BackendResponse
{
    using System.Collections.Generic;

    public class CacheStatusList : List<CacheStatus> {}

    public class CacheStatus
    {
        public string Name { get; set; }
        public long NumberOfRecordsToProcess { get; set; }
    }
}
