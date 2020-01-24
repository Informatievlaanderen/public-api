namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using RestSharp;
    using RestSharp.Authenticators;
    using RestSharp.Deserializers;
    using RestSharp.Serialization;

    public class TraceRestClient : IRestClient
    {
        private const string DefaultServiceName = "rest";
        private const string TypeName = "web";

        private string ServiceName { get; }

        private readonly ISpanSource _spanSource;
        private readonly IRestClient _restClient;

        public CookieContainer CookieContainer
        {
            get => _restClient.CookieContainer;
            set => _restClient.CookieContainer = value;
        }

        public bool AutomaticDecompression
        {
            get => _restClient.AutomaticDecompression;
            set => _restClient.AutomaticDecompression = value;
        }

        public int? MaxRedirects
        {
            get => _restClient.MaxRedirects;
            set => _restClient.MaxRedirects = value;
        }

        public string UserAgent
        {
            get => _restClient.UserAgent;
            set => _restClient.UserAgent = value;
        }

        public int Timeout
        {
            get => _restClient.Timeout;
            set => _restClient.Timeout = value;
        }

        public int ReadWriteTimeout
        {
            get => _restClient.ReadWriteTimeout;
            set => _restClient.ReadWriteTimeout = value;
        }

        public bool UseSynchronizationContext
        {
            get => _restClient.UseSynchronizationContext;
            set => _restClient.UseSynchronizationContext = value;
        }

        public IAuthenticator Authenticator
        {
            get => _restClient.Authenticator;
            set => _restClient.Authenticator = value;
        }

        public Uri BaseUrl
        {
            get => _restClient.BaseUrl;
            set => _restClient.BaseUrl = value;
        }

        public Encoding Encoding
        {
            get => _restClient.Encoding;
            set => _restClient.Encoding = value;
        }

        public bool ThrowOnDeserializationError
        {
            get => _restClient.ThrowOnDeserializationError;
            set => _restClient.ThrowOnDeserializationError = value;
        }
        public bool FailOnDeserializationError
        {
            get => _restClient.FailOnDeserializationError;
            set => _restClient.FailOnDeserializationError = value;
        }

        public bool ThrowOnAnyError
        {
            get => _restClient.ThrowOnAnyError;
            set => _restClient.ThrowOnAnyError = value;
        }

        public string ConnectionGroupName
        {
            get => _restClient.ConnectionGroupName;
            set => _restClient.ConnectionGroupName = value;
        }

        public bool PreAuthenticate
        {
            get => _restClient.PreAuthenticate;
            set => _restClient.PreAuthenticate = value;
        }

        public bool UnsafeAuthenticatedConnectionSharing
        {
            get => _restClient.UnsafeAuthenticatedConnectionSharing;
            set => _restClient.UnsafeAuthenticatedConnectionSharing = value;
        }

        public IList<Parameter> DefaultParameters => _restClient.DefaultParameters;

        public string BaseHost
        {
            get => _restClient.BaseHost;
            set => _restClient.BaseHost = value;
        }

        public bool AllowMultipleDefaultParametersWithSameName
        {
            get => _restClient.AllowMultipleDefaultParametersWithSameName;
            set => _restClient.AllowMultipleDefaultParametersWithSameName = value;
        }

        public X509CertificateCollection ClientCertificates
        {
            get => _restClient.ClientCertificates;
            set => _restClient.ClientCertificates = value;
        }

        public IWebProxy Proxy
        {
            get => _restClient.Proxy;
            set => _restClient.Proxy = value;
        }

        public RequestCachePolicy CachePolicy
        {
            get => _restClient.CachePolicy;
            set => _restClient.CachePolicy = value;
        }

        public bool Pipelined
        {
            get => _restClient.Pipelined;
            set => _restClient.Pipelined = value;
        }

        public bool FollowRedirects
        {
            get => _restClient.FollowRedirects;
            set => _restClient.FollowRedirects = value;
        }

        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
        {
            get => _restClient.RemoteCertificateValidationCallback;
            set => _restClient.RemoteCertificateValidationCallback = value;
        }

        public TraceRestClient(IRestClient restClient)
            : this(restClient, DefaultServiceName, TraceContextSpanSource.Instance) { }

        public TraceRestClient(IRestClient restClient, string serviceName)
            : this(restClient, serviceName, TraceContextSpanSource.Instance) { }

        public TraceRestClient(IRestClient restClient, ISpanSource spanSource)
            : this(restClient, DefaultServiceName, spanSource) { }

        public TraceRestClient(IRestClient restClient, string serviceName, ISpanSource spanSource)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public Task<IRestResponse> ExecutePostAsync(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecutePostAsync(request, cancellationToken);
        }

        [Obsolete("Use the overload that accepts the delegate factory")]
        public IRestClient UseSerializer(IRestSerializer serializer) => _restClient.UseSerializer(serializer);

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            return _restClient.ExecuteAsync(request, callback);
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            return _restClient.ExecuteAsync(request, callback);
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _restClient.ExecuteAsync(request, callback, httpMethod);
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _restClient.ExecuteAsync(request, callback, httpMethod);
        }

        public IRestClient UseSerializer(Func<IRestSerializer> serializerFactory)
        {
            return _restClient.UseSerializer(serializerFactory);
        }

        public IRestClient UseSerializer<T>() where T : IRestSerializer, new()
        {
            return _restClient.UseSerializer<T>();
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            return _restClient.Deserialize<T>(response);
        }

        public IRestClient UseUrlEncoder(Func<string, string> encoder)
        {
            return _restClient.UseUrlEncoder(encoder);
        }

        public IRestClient UseQueryEncoder(Func<string, Encoding, string> queryEncoder)
        {
            return _restClient.UseQueryEncoder(queryEncoder);
        }

        public IRestResponse Execute(IRestRequest request)
        {
            return _restClient.Execute(request);
        }

        public IRestResponse Execute(IRestRequest request, Method httpMethod)
        {
            return _restClient.Execute(request, httpMethod);
        }

        public IRestResponse<T> Execute<T>(IRestRequest request)
        {
            return _restClient.Execute<T>(request);
        }

        public IRestResponse<T> Execute<T>(IRestRequest request, Method httpMethod)
        {
            return _restClient.Execute<T>(request, httpMethod);
        }

        public byte[] DownloadData(IRestRequest request)
        {
            return _restClient.DownloadData(request);
        }

        public byte[] DownloadData(IRestRequest request, bool throwOnError)
        {
            return _restClient.DownloadData(request, throwOnError);
        }

        public Uri BuildUri(IRestRequest request)
        {
            return _restClient.BuildUri(request);
        }

        public string BuildUriWithoutQueryParameters(IRestRequest request)
        {
            return _restClient.BuildUriWithoutQueryParameters(request);
        }

        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _restClient.ExecuteAsyncGet(request, callback, httpMethod);
        }

        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _restClient.ExecuteAsyncPost(request, callback, httpMethod);
        }

        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _restClient.ExecuteAsyncGet(request, callback, httpMethod);
        }

        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _restClient.ExecuteAsyncPost(request, callback, httpMethod);
        }

        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            _restClient.ConfigureWebRequest(configurator);
        }

        [Obsolete("Use the overload that accepts a factory delegate")]
        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            _restClient.AddHandler(contentType, deserializer);
        }

        public void AddHandler(string contentType, Func<IDeserializer> deserializerFactory)
        {
            _restClient.AddHandler(contentType, deserializerFactory);
        }

        public void RemoveHandler(string contentType)
        {
            _restClient.RemoveHandler(contentType);
        }

        public void ClearHandlers()
        {
            _restClient.ClearHandlers();
        }

        public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
        {
            return _restClient.ExecuteAsGet(request, httpMethod);
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            return _restClient.ExecuteAsPost(request, httpMethod);
        }

        public Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync<T>(request, cancellationToken);
        }

        public Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync<T>(request, httpMethod, cancellationToken);
        }

        public Task<IRestResponse> ExecuteAsync(IRestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteAsync(request, httpMethod, cancellationToken);

        }

        public Task<IRestResponse<T>> ExecuteGetAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteGetAsync<T>(request, cancellationToken);
        }

        public Task<IRestResponse<T>> ExecutePostAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecutePostAsync<T>(request, cancellationToken);
        }

        public Task<IRestResponse> ExecuteGetAsync(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _restClient.ExecuteGetAsync(request, cancellationToken);
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod)
        {
            return _restClient.ExecuteAsGet<T>(request, httpMethod);
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod)
        {
            return _restClient.ExecuteAsPost<T>(request, httpMethod);
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            const string name = "rest." + nameof(ExecuteTaskAsync);
            var span = _spanSource.Begin(name, ServiceName, BuildResource(request), TypeName);
            try
            {
                span?.SetMeta("http.method", request.Method.ToString());
                span?.SetMeta("http.path", request.Resource);

                if (span != null)
                {
                    request.AddHeader(DataDogOptions.DefaultTraceIdHeaderName, span.TraceId.ToString());
                    request.AddHeader(DataDogOptions.DefaultParentSpanIdHeaderName, span.SpanId.ToString());
                }

                var response = await _restClient.ExecuteTaskAsync<T>(request, token);

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

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, Method httpMethod)
        {
            return _restClient.ExecuteTaskAsync<T>(request, httpMethod);
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            return _restClient.ExecuteTaskAsync<T>(request);
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            return _restClient.ExecuteGetTaskAsync<T>(request);
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            return _restClient.ExecuteGetTaskAsync<T>(request, token);
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            return _restClient.ExecutePostTaskAsync<T>(request);
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            return _restClient.ExecutePostTaskAsync<T>(request, token);
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            const string name = "rest." + nameof(ExecuteTaskAsync);
            var span = _spanSource.Begin(name, ServiceName, BuildResource(request), TypeName);
            try
            {
                span?.SetMeta("http.method", request.Method.ToString());
                span?.SetMeta("http.path", request.Resource);

                if (span != null)
                {
                    request.AddHeader(DataDogOptions.DefaultTraceIdHeaderName, span.TraceId.ToString());
                    request.AddHeader(DataDogOptions.DefaultParentSpanIdHeaderName, span.SpanId.ToString());
                }

                var response = await _restClient.ExecuteTaskAsync(request, token);

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

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token, Method httpMethod)
        {
            return _restClient.ExecuteTaskAsync(request, token, httpMethod);
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            return _restClient.ExecuteTaskAsync(request);
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            return _restClient.ExecuteGetTaskAsync(request);
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            return _restClient.ExecuteGetTaskAsync(request, token);
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            return _restClient.ExecutePostTaskAsync(request);
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            return _restClient.ExecutePostTaskAsync(request, token);
        }

        private string BuildResource(IRestRequest restRequest) => string.Concat(BaseUrl, "/", restRequest.Resource);
    }
}
