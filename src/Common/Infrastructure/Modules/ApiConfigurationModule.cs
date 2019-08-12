namespace Common.Infrastructure.Modules
{
    using System;
    using System.Net;
    using Autofac;
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
        private readonly ApiConfigurationSection _apiConfiguration = new ApiConfigurationSection();

        public ApiConfigurationModule(IConfiguration configuration)
        {
            _downstreamUser = configuration["RegistryAuthUser"];
            _downstreamPass = configuration["RegistryAuthPass"];

            _serviceName = configuration["DataDog:ServiceName"];

            configuration.GetSection("ApiConfiguration").Bind(_apiConfiguration);
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var registry in _apiConfiguration)
            {
                RegisterRestClient(registry.Key, registry.Value.ApiUrl, _downstreamUser, _downstreamPass, _serviceName, builder);
                RegisterApiCacheToggle(registry.Key, registry.Value.UseCache, builder);
            }
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

        private static void RegisterApiCacheToggle(string name, bool toggleValue, ContainerBuilder builder)
            => builder
                .Register(c =>
                {
                    var useRedis = c.Resolve<ApiRedisToggle>();
                    return new ApiCacheToggle(useRedis.FeatureEnabled && toggleValue);
                })
                .Keyed<IFeatureToggle>(name);
    }
}
