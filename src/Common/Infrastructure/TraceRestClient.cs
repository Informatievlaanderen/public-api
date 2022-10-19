namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Http.Headers;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using RestSharp;
    using RestSharp.Authenticators;
    using RestSharp.Serializers;

    public class TraceRestClient : RestClient
    {
        private const string DefaultServiceName = "rest";
        private const string TypeName = "web";

        private string ServiceName { get; }

        private readonly ISpanSource _spanSource;
        private readonly RestClient _restClient;

        public CookieContainer? CookieContainer
        {
            get => _restClient.Options.CookieContainer;
            set => _restClient.Options.CookieContainer = value;
        }

        public DecompressionMethods AutomaticDecompression
        {
            get => _restClient.Options.AutomaticDecompression;
            set => _restClient.Options.AutomaticDecompression = value;
        }

        public int? MaxRedirects
        {
            get => _restClient.Options.MaxRedirects;
            set => _restClient.Options.MaxRedirects = value;
        }

        public string UserAgent
        {
            get => _restClient.Options.UserAgent;
            set => _restClient.Options.UserAgent = value;
        }

        public int Timeout
        {
            get => _restClient.Options.MaxTimeout;
            set => _restClient.Options.MaxTimeout = value;
        }

        public IAuthenticator? Authenticator
        {
            get => _restClient.Authenticator;
            set => _restClient.Authenticator = value;
        }

        public Uri? BaseUrl
        {
            get => _restClient.Options.BaseUrl;
            set => _restClient.Options.BaseUrl = value;
        }

        public Encoding Encoding
        {
            get => _restClient.Options.Encoding;
            set => _restClient.Options.Encoding = value;
        }

        public bool ThrowOnDeserializationError
        {
            get => _restClient.Options.ThrowOnDeserializationError;
            set => _restClient.Options.ThrowOnDeserializationError = value;
        }

        public bool FailOnDeserializationError
        {
            get => _restClient.Options.FailOnDeserializationError;
            set => _restClient.Options.FailOnDeserializationError = value;
        }

        public bool ThrowOnAnyError
        {
            get => _restClient.Options.ThrowOnAnyError;
            set => _restClient.Options.ThrowOnAnyError = value;
        }

        public bool PreAuthenticate
        {
            get => _restClient.Options.PreAuthenticate;
            set => _restClient.Options.PreAuthenticate = value;
        }

        public ParametersCollection DefaultParameters => _restClient.DefaultParameters;

        public string? BaseHost
        {
            get => _restClient.Options.BaseHost;
            set => _restClient.Options.BaseHost = value;
        }

        public bool AllowMultipleDefaultParametersWithSameName
        {
            get => _restClient.Options.AllowMultipleDefaultParametersWithSameName;
            set => _restClient.Options.AllowMultipleDefaultParametersWithSameName = value;
        }

        public X509CertificateCollection? ClientCertificates
        {
            get => _restClient.Options.ClientCertificates;
            set => _restClient.Options.ClientCertificates = value;
        }

        public IWebProxy? Proxy
        {
            get => _restClient.Options.Proxy;
            set => _restClient.Options.Proxy = value;
        }

        public CacheControlHeaderValue? CachePolicy
        {
            get => _restClient.Options.CachePolicy;
            set => _restClient.Options.CachePolicy = value;
        }

        public bool FollowRedirects
        {
            get => _restClient.Options.FollowRedirects;
            set => _restClient.Options.FollowRedirects = value;
        }

        public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback
        {
            get => _restClient.Options.RemoteCertificateValidationCallback;
            set => _restClient.Options.RemoteCertificateValidationCallback = value;
        }

        public TraceRestClient(RestClient restClient)
            : this(restClient, DefaultServiceName, TraceContextSpanSource.Instance) { }

        public TraceRestClient(RestClient restClient, string serviceName)
            : this(restClient, serviceName, TraceContextSpanSource.Instance) { }

        public TraceRestClient(RestClient restClient, ISpanSource spanSource)
            : this(restClient, DefaultServiceName, spanSource) { }

        public TraceRestClient(RestClient restClient, string serviceName, ISpanSource spanSource)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public Task<RestResponse> ExecutePostAsync(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecutePostAsync(request, cancellationToken);
        }

        public RestClient UseSerializer(Func<IRestSerializer> serializerFactory)
        {
            return _restClient.UseSerializer(serializerFactory);
        }

        public RestClient UseSerializer<T>() where T : class, IRestSerializer, new()
        {
            return _restClient.UseSerializer<T>();
        }

        public RestResponse<T> Deserialize<T>(RestResponse response)
        {
            return _restClient.Deserialize<T>(response);
        }

        public RestClient UseUrlEncoder(Func<string, string> encoder)
        {
            return _restClient.UseUrlEncoder(encoder);
        }

        public RestClient UseQueryEncoder(Func<string, Encoding, string> queryEncoder)
        {
            return _restClient.UseQueryEncoder(queryEncoder);
        }

        public RestResponse Execute(RestRequest request)
        {
            return _restClient.Execute(request);
        }

        public RestResponse Execute(RestRequest request, Method httpMethod)
        {
            return _restClient.Execute(request, httpMethod);
        }

        public RestResponse<T> Execute<T>(RestRequest request)
        {
            return _restClient.Execute<T>(request);
        }

        public RestResponse<T> Execute<T>(RestRequest request, Method httpMethod)
        {
            return _restClient.Execute<T>(request, httpMethod);
        }

        public byte[] DownloadData(RestRequest request)
        {
            return _restClient.DownloadData(request);
        }

        public Uri BuildUri(RestRequest request)
        {
            return _restClient.BuildUri(request);
        }

        public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync<T>(request, cancellationToken);
        }

        public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync<T>(request, httpMethod, cancellationToken);
        }

        public Task<RestResponse> ExecuteAsync(RestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync(request, httpMethod, cancellationToken);
        }

        public Task<RestResponse<T>> ExecuteGetAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteGetAsync<T>(request, cancellationToken);
        }

        public Task<RestResponse<T>> ExecutePostAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecutePostAsync<T>(request, cancellationToken);
        }

        public Task<RestResponse> ExecuteGetAsync(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteGetAsync(request, cancellationToken);
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

        [Obsolete("Use the overload that accepts the delegate factory")]
        public RestClient UseSerializer(IRestSerializer serializer) => _restClient.UseSerializer(() => serializer);

        private string BuildResource(RestRequest restRequest) => string.Concat(BaseUrl, "/", restRequest.Resource);
    }
}
