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
        private readonly ApiConfigurationSection _apiConfiguration = new ApiConfigurationSection();

        public ApiConfigurationModule(IConfiguration configuration)
        {
            _downstreamUser = configuration["RegistryAuthUser"];
            _downstreamPass = configuration["RegistryAuthPass"];

            configuration.GetSection("ApiConfiguration").Bind(_apiConfiguration);
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var registry in _apiConfiguration)
            {
                RegisterRestClient(registry.Key, registry.Value.ApiUrl, _downstreamUser, _downstreamPass, builder);
                RegisterApiCacheToggle(registry.Key, registry.Value.UseCache, builder);
            }
        }

        internal static void RegisterRestClient(
            string name,
            string baseUrl,
            string user,
            string password,
            ContainerBuilder builder)
            => builder
                .RegisterType<RestClient>()
                .WithProperty("BaseUrl", new Uri(baseUrl))
                .WithProperty("CookieContainer", new CookieContainer())
                .WithProperty("Authenticator", new HttpBasicAuthenticator(user, password))
                .Keyed<IRestClient>(name);

        internal static void RegisterApiCacheToggle(string name, bool toggleValue, ContainerBuilder builder)
            => builder
                .Register(c =>
                {
                    var useRedis = c.Resolve<ApiRedisToggle>();
                    return new ApiCacheToggle(useRedis.FeatureEnabled && toggleValue);
                })
                .Keyed<IFeatureToggle>(name);
    }
}
