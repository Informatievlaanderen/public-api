namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class CacheStatusClient : BaseStatusClient<IEnumerable<RegistryCacheStatus>, CacheStatusList>
    {
        public CacheStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("caches");

        protected override IEnumerable<RegistryCacheStatus> Map(CacheStatusList response)
            => response?
                .Select(status =>
                    new RegistryCacheStatus
                    {
                        Name = status.Name,
                        NumberOfRecordsToProcess = status.NumberOfRecordsToProcess
                    })
                .Where(status => status != null)
               ?? new List<RegistryCacheStatus>();
    }
}
