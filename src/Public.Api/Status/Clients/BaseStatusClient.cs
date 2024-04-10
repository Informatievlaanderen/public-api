namespace Public.Api.Status.Clients
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using RestSharp;

    public  abstract class BaseStatusClient<TStatus, TRestResponse> : IStatusClient<TStatus>
    {
        private readonly RestClient _restClient;

        public string Registry { get; }

        protected BaseStatusClient(string registry, RestClient restClient)
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

        protected abstract RestRequest CreateStatusRequest();
        protected abstract TStatus Map(TRestResponse response);
    }
}
