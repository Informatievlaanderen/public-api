namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.AttributeFilters;
    using Common.Infrastructure;
    using Common.Infrastructure.Modules;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Swagger;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Feeds;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private IContainer _applicationContainer;

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureDefaultForApi<Startup>(
                    (provider, description) => new Info
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "Basisregisters Vlaanderen API",
                        Description = GetApiLeadingText(description),
                        Contact = new Contact
                        {
                            Name = "Informatie Vlaanderen",
                            Email = "informatie.vlaanderen@vlaanderen.be",
                            Url = "https://legacy.basisregisters.vlaanderen"
                        }
                    },
                    new []
                    {
                        typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                        typeof(MunicipalityRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                    },
                    _configuration.GetSection("Cors").GetChildren().Select(c => c.Value).ToArray())

                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()

                .AddHttpCacheHeaders(
                    expirationModelOptions =>
                    {
                        expirationModelOptions.CacheLocation = CacheLocation.Private;
                        expirationModelOptions.NoStore = false;
                        expirationModelOptions.NoTransform = true;

                        expirationModelOptions.MaxAge = 12 * 60 * 60; // Hours, Minutes, Second
                        expirationModelOptions.SharedMaxAge = 12 * 60 * 60; // Hours, Minutes, Second
                    },
                    validationModelOptions =>
                    {
                        validationModelOptions.NoCache = false;
                        validationModelOptions.MustRevalidate = true;
                        validationModelOptions.ProxyRevalidate = true;

                        validationModelOptions.VaryByAll = false;
                        validationModelOptions.Vary = new List<string> { "Accept", "Accept-Encoding" };
                    },
                    storeKeyGeneratorFunc: _ => new RedisStoreKeyGenerator(_loggerFactory.CreateLogger<RedisStoreKeyGenerator>()),
                    validatorValueStoreFunc: x =>
                    {
                        var redisProvider = x.GetService<ConnectionMultiplexerProvider>();
                        var redis = redisProvider.GetConnectionMultiplexer();

                        return redis != null
                            ? new RedisStore(_loggerFactory.CreateLogger<RedisStore>(), redis) as IValidatorValueStore
                            : new InMemoryValidatorValueStore(_loggerFactory.CreateLogger<InMemoryValidatorValueStore>()) as IValidatorValueStore;
                    })

                .Configure<MunicipalityRegistry.Api.Legacy.Infrastructure.Options.ResponseOptions>(_configuration.GetSection("ApiConfiguration:MunicipalityRegistry"));

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new ApiConfigurationModule(_configuration));
            containerBuilder.RegisterModule(new DataDogModule(_configuration));
            containerBuilder.RegisterModule(new RedisModule(_configuration));

            containerBuilder.Populate(services);

            containerBuilder
                .RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Registry.Api")).ToArray())
                .AssignableTo<IExamplesProvider>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsSubClassOfGeneric(typeof(RegistryApiController<>)))
                .WithAttributeFiltering();

            containerBuilder.RegisterType<FeedController>().WithAttributeFiltering();

            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            ApiDataDogToggle datadogToggle,
            ApiDebugDataDogToggle debugDataDogToggle)
        {
            if (datadogToggle.FeatureEnabled)
            {
                if (debugDataDogToggle.FeatureEnabled)
                    StartupHelpers.SetupSourceListener(serviceProvider.GetRequiredService<TraceSource>());

                app.UseDataDogTracing(
                    serviceProvider.GetRequiredService<TraceSource>(),
                    _configuration["DataDog:ServiceName"],
                    pathToCheck => pathToCheck != "/");
            }

            app.UseDefaultForApi(new StartupOptions
            {
                ApplicationContainer = _applicationContainer,
                ServiceProvider = serviceProvider,
                HostingEnvironment = env,
                ApplicationLifetime = appLifetime,
                LoggerFactory = loggerFactory,
                Api =
                {
                    VersionProvider = apiVersionProvider,
                    Info = groupName => $"Basisregisters Vlaanderen - Base Registries API {groupName}"
                },
                MiddlewareHooks =
                {
                    AfterResponseCompression = x => x.UseHttpCacheHeaders(),
                }
            });
        }

        private string GetApiLeadingText(ApiVersionDescription description)
        {
            var text = new StringBuilder(1000);

            text.Append(
$@"# Introductie

Welkom bij de REST API van Basisregisters Vlaanderen!

[REST](http://en.wikipedia.org/wiki/REST_API) is een webserviceprotocol dat zich leent voor snelle ontwikkeling door het gebruik van HTTP- en JSON-technologie.

Basisregisters Vlaanderen stelt u in staat om alles te weten te komen rond:
* de Belgische gemeenten;
* de Belgische postcodes;
* de Vlaamse straatnamen;
* de Vlaamse adressen;
* de Vlaamse gebouwen en gebouweenheden;
* de Vlaamse percelen;
* de Vlaamse organisaties en organen;
* de Vlaamse dienstverlening;

Basisregisters Vlaanderen is de authentieke bron rond al bovenstaande gegevens met uitzondering van gemeenten, postcodes en percelen, die wij aanbieden als referentie bron.

## Contact

U kan ons bereiken via [informatie.vlaanderen@vlaanderen.be](mailto:informatie.vlaanderen@vlaanderen.be).

## Ontsluitingen

De REST API van Basisregisters Vlaanderen is te bereiken via volgende ontsluitingen.

Doelpubliek | Basis URL voor de REST ontsluitingen                              |
----------- | ----------------------------------------------------------------- |
Iedereen    | {_configuration["BaseUrl"]}{description.GroupName} |

## Toegang tot de API

U kan anoniem gebruik maken van de API, echter is deze beperkt in het aantal verzoeken dat u tegelijk kan sturen.

Wenst u volwaardige toegang tot de api, dan kan u zich [hier aanmelden als ontwikkelaar](https://portal.basisregisters.vlaanderen).

# Authenticatie

# Verzoeken en Responsen

## Foutmeldingen

De Basisregisters Vlaanderen API gebruikt [Problem Details for HTTP APIs (RFC7807)](https://tools.ietf.org/html/rfc7807) om foutmeldingen te ontsluiten. Een foutmelding zal resulteren in volgende datastructuur:

```
{{
  ""type"": ""string"",
  ""title"": ""string"",
  ""detail"": ""string"",
  ""status"": number,
  ""instance"": ""string""
}}
```

# Paginering

# Versionering

Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen API{String.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund**." : ".")}");

            return text.ToString();
        }
    }
}
