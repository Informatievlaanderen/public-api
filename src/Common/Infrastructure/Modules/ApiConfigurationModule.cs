namespace Common.Infrastructure.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Net;
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
                RegisterRestClient(registry.Key, registry.Value.ApiUrl, _downstreamUser, _downstreamPass, _serviceName, builder);
                RegisterHealthClient(registry.Key, registry.Value.HealthUrl, _downstreamUser, _downstreamPass, _serviceName, builder);
                RegisterApiCacheToggle(registry.Key, registry.Value.UseCache, builder);

                healthUrls.Add(registry.Key, registry.Value.HealthUrl);
            }

            builder.RegisterInstance(healthUrls);
            builder.RegisterType<ProblemDetailsHelper>();
        }

        private static void RegisterRestClient(
            string name,
            string baseUrl,
            string user,
            string password,
            string serviceName,
            ContainerBuilder builder)
        {
            builder
                .RegisterType<RestClient>()
                .WithProperty("BaseUrl", new Uri(baseUrl))
                .WithProperty("CookieContainer", new CookieContainer())
                .WithProperty("Authenticator", new HttpBasicAuthenticator(user, password))
                .Keyed<RestClient>(name);

            builder
                .Register(context => new TraceRestClient(context.ResolveNamed<RestClient>(name), serviceName))
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
                .RegisterType<RestClient>()
                .WithProperty("BaseUrl", new Uri(baseUrl))
                .WithProperty("CookieContainer", new CookieContainer())
                .WithProperty("Authenticator", new HttpBasicAuthenticator(user, password))
                .Keyed<RestClient>(healthServiceName);

            builder
                .Register(context => new TraceRestClient(context.ResolveNamed<RestClient>(healthServiceName), serviceName))
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

    public class HealthUrls : Dictionary<string, string> { }
}
