namespace Common.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using RestSharp;
    using RestSharp.Serializers;

    public interface IRestClient
    {
        Task<RestResponse> ExecuteAsync(
            RestRequest request,
            CancellationToken cancellationToken = new());
    }

    public class TraceRestClient : IRestClient
    {
        private const string DefaultServiceName = "rest";
        private const string TypeName = "web";

        private string ServiceName { get; }

        private readonly ISpanSource _spanSource;
        private readonly RestClient _restClient;

        private Uri? BaseUrl => _restClient.Options.BaseUrl;

        public TraceRestClient(RestClient restClient, string serviceName)
            : this(restClient, serviceName, TraceContextSpanSource.Instance) { }

        private TraceRestClient(RestClient restClient, string serviceName, ISpanSource spanSource)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public RestClient UseSerializer<T>() where T : class, IRestSerializer, new()
        {
            return _restClient.UseSerializer<T>();
        }

        public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync<T>(request, cancellationToken);
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken)
        {
            const string name = "rest." + nameof(ExecuteAsync);
            var span = _spanSource.Begin(name, ServiceName, BuildResource(request), TypeName);
            try
            {
                if (span is not null)
                {
                    span.SetMeta("http.method", request.Method.ToString());
                    span.SetMeta("http.path", request.Resource);

                    request.AddHeader(DataDogOptions.DefaultTraceIdHeaderName, span.TraceId.ToString());
                    request.AddHeader(DataDogOptions.DefaultParentSpanIdHeaderName, span.SpanId.ToString());
                }

                var response = await _restClient.ExecuteAsync(request, cancellationToken);

                span?.SetMeta("http.status_code", response.StatusCode.ToString());

                return response;
            }
            catch (Exception ex)
            {
                span?.SetError(ex);
                throw;
            }
            finally
            {
                span?.Dispose();
            }
        }

        private string BuildResource(RestRequest restRequest) => string.Concat(BaseUrl, "/", restRequest.Resource);
    }
}
