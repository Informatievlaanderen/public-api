namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class CacheStatusClient : BaseStatusClient<IEnumerable<RegistryCacheStatus>, IEnumerable<CacheStatusClient.CacheStatus>>
    {
        public CacheStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient)
        { }

        protected override IRestRequest CreateStatusRequest()
            => new RestRequest("caches");

        protected override IEnumerable<RegistryCacheStatus> Map(IEnumerable<CacheStatus> response)
            => response?
                .Select(status =>
                    new RegistryCacheStatus
                    {
                        Name = status.Name,
                        NumberOfRecordsToProcess = status.NumberOfRecordsToProcess
                    })
                .Where(status => status != null)
               ?? new List<RegistryCacheStatus>();

        public class CacheStatus
        {
            public string Name { get; set; }
            public long NumberOfRecordsToProcess { get; set; }
        }
    }
}
