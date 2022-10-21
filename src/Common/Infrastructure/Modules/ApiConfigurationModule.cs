namespace Common.Infrastructure.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Text;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Configuration;
    using FeatureToggle;
    using Microsoft.Extensions.Configuration;
    using RestSharp;
    using RestSharp.Authenticators;

    public class ApiConfigurationModule : Module
    {
        private readonly string _downstreamUser;
        private readonly string _downstreamPass;
        private readonly string _serviceName;
        private readonly NamedConfigurations<ApiConfiguration> _apiConfiguration;

        public ApiConfigurationModule(IConfiguration configuration)
        {
            _downstreamUser = configuration["RegistryAuthUser"];
            _downstreamPass = configuration["RegistryAuthPass"];

            _serviceName = configuration["DataDog:ServiceName"];

            _apiConfiguration = new NamedConfigurations<ApiConfiguration>(configuration, "ApiConfiguration");
        }

        protected override void Load(ContainerBuilder builder)
        {
            var healthUrls = new HealthUrls();

            foreach (var registry in _apiConfiguration)
            {
                RegisterRestClient(registry.Key, registry.Value.ApiUrl, _serviceName, builder);
                RegisterHttpClient(registry.Key, registry.Value.ApiUrl, builder);
                RegisterHealthClient(registry.Key, registry.Value.HealthUrl, _downstreamUser, _downstreamPass, _serviceName, builder);
                RegisterApiCacheToggle(registry.Key, registry.Value.UseCache, builder);

                healthUrls.Add(registry.Key, registry.Value.HealthUrl);
            }

            builder.RegisterInstance(healthUrls);
            builder
                .RegisterType<ProblemDetailsHelper>()
                .AsSelf();
        }

        private void RegisterHttpClient(
            string name,
            string valueApiUrl,
            ContainerBuilder builder)
        {
            builder
                .Register<HttpClient>(c =>
                {
                    var client = c.Resolve<IHttpClientFactory>().CreateClient();
                    client.BaseAddress = new Uri(valueApiUrl.EndsWith("/") ? valueApiUrl : valueApiUrl + "/");
                    return client;
                })
                .As<HttpClient>()
                .Keyed<HttpClient>(name);
        }

        private static void RegisterRestClient(
            string name,
            string baseUrl,
            string serviceName,
            ContainerBuilder builder)
        {
            builder
                .Register(context =>
                {
                    var restClient = new RestClient(new RestClientOptions(new Uri(baseUrl))
                    {
                        CookieContainer = new CookieContainer(),
                        Encoding = Encoding.UTF8
                    });

                    var traceRestClient = new TraceRestClient(restClient, serviceName);
                    traceRestClient.UseSerializer<JsonNetSerializer>();
                    return traceRestClient;
                })
                .Keyed<TraceRestClient>(name)
                .Keyed<IRestClient>(name);
        }

        private static void RegisterHealthClient(
            string name,
            string baseUrl,
            string user,
            string password,
            string serviceName,
            ContainerBuilder builder)
        {
            var healthServiceName = $"Health-{name}";

            builder
                .Register(context =>
                {
                    var restClient = new RestClient(
                            new RestClientOptions(new Uri(baseUrl))
                            {
                                CookieContainer = new CookieContainer(),
                                Encoding = Encoding.UTF8
                            })
                    {
                        Authenticator = new HttpBasicAuthenticator(user, password)
                    };

                    return new TraceRestClient(restClient, serviceName);
                })
                .Keyed<TraceRestClient>(healthServiceName)
                .Keyed<IRestClient>(healthServiceName);
        }

        private static void RegisterApiCacheToggle(string name, bool toggleValue, ContainerBuilder builder)
            => builder
                .Register(c =>
                {
                    var useRedis = c.Resolve<ApiRedisToggle>();
                    return new ApiCacheToggle(useRedis.FeatureEnabled && toggleValue);
                })
                .Keyed<IFeatureToggle>(name);
    }

    [Serializable]
    public sealed class HealthUrls : Dictionary<string, string>
    {
        public HealthUrls()
        { }

        private HealthUrls(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
