namespace Public.Api.Status.Clients
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using RestSharp;

    public  abstract class BaseStatusClient<TStatus, TRestResponse> : IStatusClient<TStatus>
    {
        private readonly TraceRestClient _restClient;

        public string Registry { get; }

        protected BaseStatusClient(string registry, TraceRestClient restClient)
        {
            if (string.IsNullOrWhiteSpace(registry))
                throw new ArgumentNullException(nameof(registry));
                
            Registry = registry;
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public async Task<TStatus> GetStatus(CancellationToken cancellationToken)
        {
            var response = await _restClient.ExecuteAsync<TRestResponse>(CreateStatusRequest(), cancellationToken);

            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK && response.Data != null)
                return Map(response.Data);

            return default;
        }

        protected abstract IRestRequest CreateStatusRequest();
        protected abstract TStatus Map(TRestResponse response);
    }
}
