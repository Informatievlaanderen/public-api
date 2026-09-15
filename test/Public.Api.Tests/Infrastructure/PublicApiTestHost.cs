namespace Public.Api.Tests.Infrastructure
{
    using System.Net;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Public.Api.Infrastructure;
    using Public.Api.Infrastructure.Configuration;
    using RestSharp;

    /// <summary>
    /// The public API as the host composes it - its configuration, its container registrations and its whole middleware
    /// pipeline - on a test server. Two things are replaced: the feature toggles, which are stored in DynamoDB and are
    /// all enabled here, and the backend of the road registry, which answers what a test tells it to.
    /// </summary>
    /// <remarks>
    /// Started once for all tests, which share it through <see cref="PublicApiCollection"/>: the host registers the
    /// response examples of every registry assembly loaded at that moment, and a second host in the same process would
    /// find assemblies loaded by the first one that it cannot scan.
    /// </remarks>
    public sealed class PublicApiTestHost : IAsyncLifetime
    {
        private IHost _host = null!;

        public FakeBackend RoadRegistry { get; } = new();

        public IServiceProvider Services => _host.Services;

        public HttpClient CreateClient() => _host.GetTestClient();

        public async Task InitializeAsync()
        {
            var roadRegistry = RoadRegistry;

            _host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(configuration => configuration
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    // The road segment requests bypass the HTTP cache headers middleware. It buffers the response body in
                    // a stream it disposes of without putting the original one back when the request fails, so on a test
                    // server the exception handler cannot write the problem details of a backend 404 - which the public
                    // API answers as expected when it is hosted for real.
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ExcludedRoutes:Routes:1"] = "/wegsegmenten/"
                    }))
                .ConfigureContainer<ContainerBuilder>((hostContext, containerBuilder) =>
                {
                    Program.ConfigureContainer(hostContext.Configuration, containerBuilder);

                    // Registered last, so it replaces the one ApiConfigurationModule made for the road registry.
                    containerBuilder
                        .Register(_ => new RestClient(
                            new RestClientOptions(new Uri("http://road-registry.test/v1"))
                            {
                                ConfigureMessageHandler = _ => roadRegistry
                            },
                            configureSerialization: serialization => serialization.UseSerializer<JsonNetSerializer>()))
                        .Keyed<RestClient>(RegistryKeys.Road);
                })
                .ConfigureWebHost(webHost => webHost
                    .UseTestServer()
                    .UseEnvironment("Test")
                    .UseContentRoot(AppContext.BaseDirectory)
                    .UseStartup<TestStartup>()
                    // After UseStartup, which names the application after the assembly the test startup is in: the
                    // application is Public.Api, as for the real host, and MVC finds the controllers in its assembly.
                    .UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name))
                .Build();

            await _host.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        private sealed class TestStartup : Startup
        {
            public TestStartup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
                : base(webHostEnvironment, configuration)
            {
            }

            protected override IDynamicFeatureToggleService CreateFeatureToggleService(IServiceCollection services)
                => new AllFeaturesEnabled();
        }

        private sealed class AllFeaturesEnabled : IDynamicFeatureToggleService
        {
            public bool IsFeatureEnabled(string featureName) => true;
        }
    }

    /// <summary>A backend that answers every request with the response a test gave it, and remembers the requests.</summary>
    public sealed class FakeBackend : HttpMessageHandler
    {
        private Func<HttpResponseMessage> _respond = () => new HttpResponseMessage(HttpStatusCode.NotFound);

        public List<HttpRequestMessage> Requests { get; } = new();

        public void RespondWith(Func<HttpResponseMessage> respond) => _respond = respond;

        public void Reset()
        {
            _respond = () => new HttpResponseMessage(HttpStatusCode.NotFound);
            Requests.Clear();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            var response = _respond();
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}

namespace Public.Api.Tests.Infrastructure
{
    /// <summary>The tests that share one <see cref="PublicApiTestHost"/>; they run one after the other.</summary>
    [CollectionDefinition(Name)]
    public sealed class PublicApiCollection : ICollectionFixture<PublicApiTestHost>
    {
        public const string Name = "PublicApi";
    }
}
