namespace Public.Api.Infrastructure.Modules
{
    using System;
    using System.Net;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Registration;
    using Common.Infrastructure;
    using Common.Infrastructure.Configuration;
    using Microsoft.Extensions.Configuration;
    using RestSharp;
    using Status.Clients;

    public class StatusModule : Module
    {
        private readonly NamedConfigurations<ApiStatusConfiguration> _apiStatusConfigurations;
        private readonly string _serviceName;

        public StatusModule(IConfiguration configuration)
        {
            _serviceName = configuration["DataDog:ServiceName"];
            _apiStatusConfigurations = new NamedConfigurations<ApiStatusConfiguration>(configuration, "ApiConfiguration");
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var (key, value) in _apiStatusConfigurations)
            {
                RegisterImportStatusClient(key, value.ImportUrl, builder);
                RegisterProjectionStatusClient(key, value.ProjectionsUrl, builder);
                RegisterCacheStatusClient(key, value.ProjectionsUrl, builder);
                RegisterSyndicationStatusClient(key, value.ProjectionsUrl, builder);
            }
        }

        private void RegisterImportStatusClient(
            string name,
            string baseUrl,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            var key = $"Import-{name}";
            RegisterKeyedRestClient(baseUrl, key, builder);

            builder
                .Register(context => new ImportStatusClient(name, context.ResolveNamed<TraceRestClient>(key)))
                .AsSelf();
        }

        private void RegisterProjectionStatusClient(
            string name,
            string baseUrl,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            var key = $"Projection-{name}";
            RegisterKeyedRestClient(baseUrl, key, builder);

            builder
                .Register(context => new ProjectionStatusClient(name, context.ResolveNamed<TraceRestClient>(key)))
                .AsSelf();
        }

        private void RegisterCacheStatusClient(
            string name,
            string baseUrl,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            var key = $"Cache-{name}";
            RegisterKeyedRestClient(baseUrl, key, builder);

            builder
                .Register(context => new CacheStatusClient(name, context.ResolveNamed<TraceRestClient>(key)))
                .AsSelf();
        }

        private void RegisterSyndicationStatusClient(
            string name,
            string baseUrl,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            var key = $"Syndication-{name}";
            RegisterKeyedRestClient(baseUrl, key, builder);

            builder
                .Register(context => new SyndicationStatusClient(name, context.ResolveNamed<TraceRestClient>(key)))
                .AsSelf();
        }

        private void RegisterKeyedRestClient(
            string baseUrl,
            string key,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            // builder
            //     .RegisterType<RestClient>()
            //     .WithProperty("BaseUrl", new Uri(baseUrl))
            //     .WithProperty("CookieContainer", new CookieContainer())
            //     .Keyed<RestClient>(key)
            //     .OnlyIf(IsNotRegistered<RestClient>(key));

            builder
                .Register(context =>
                {
                    var restClient = new RestClient(new RestClientOptions(new Uri(baseUrl))
                    {
                        CookieContainer = new CookieContainer()
                    });

                    var traceRestClient = new TraceRestClient(restClient, _serviceName);
                    return traceRestClient;
                })
                .Keyed<TraceRestClient>(key)
                .Keyed<IRestClient>(key)
                .OnlyIf(IsNotRegistered<TraceRestClient>(key));
        }

        private static Predicate<IComponentRegistryBuilder> IsNotRegistered<T>(string key)
            => registry => !registry.IsRegistered(new KeyedService(key, typeof(T)));
    }
}
