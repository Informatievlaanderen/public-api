namespace Public.Api.Infrastructure.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Registration;
    using Common.Infrastructure;
    using Common.Infrastructure.Configuration;
    using Common.Infrastructure.Modules;
    using Microsoft.Extensions.Configuration;
    using RestSharp;
    using RestSharp.Authenticators;
    using Status.Clients;
    using Status.Responses;

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
                .Register(context => new ImportStatusClient(context.ResolveNamed<TraceRestClient>(key)))
                .AsSelf();
        }
        
        private void RegisterKeyedRestClient(
            string baseUrl,
            string key,
            ContainerBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            builder
                .RegisterType<RestClient>()
                .WithProperty("BaseUrl", new Uri(baseUrl))
                .WithProperty("CookieContainer", new CookieContainer())
                .Keyed<RestClient>(key)
                .OnlyIf(IsNotRegistered<RestClient>(key));

            builder
                .Register(context => new TraceRestClient(context.ResolveNamed<RestClient>(key), _serviceName))
                .Keyed<TraceRestClient>(key)
                .Keyed<IRestClient>(key)
                .OnlyIf(IsNotRegistered<TraceRestClient>(key));
        }

        private static Predicate<IComponentRegistryBuilder> IsNotRegistered<T>(string key)
            => registry => !registry.IsRegistered(new KeyedService(key, typeof(T)));
    }
}
