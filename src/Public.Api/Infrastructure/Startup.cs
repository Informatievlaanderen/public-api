namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Extensions;
    using Common.Infrastructure.Modules;
    using Configuration;
    using Extract;
    using Feeds;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Modules;
    using ProblemDetailsExceptionMapping;
    using Redis;
    using Swagger;
    using Swashbuckle.AspNetCore.Filters;
    using Version;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private static readonly SHA1 Sha1 = SHA1.Create();

        private const string DefaultCulture = "en-GB";
        private const string SupportedCultures = "en-GB;en-US;en"; //"en-GB;en-US;en;nl-BE;nl";

        private IContainer _applicationContainer;

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly OpenApiContact _contact;
        private readonly MarketingVersion _marketingVersion;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _marketingVersion = new MarketingVersion(_configuration);

            _contact = new OpenApiContact
            {
                Name = _configuration["Contact:Name"],
                Email = _configuration["Contact:Email"],
                Url = new Uri(_configuration["SiteUrl"])
            };
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var baseUrl = _configuration["BaseUrl"];
            var baseUrlForExceptions = baseUrl.EndsWith("/")
                ? baseUrl.Substring(0, baseUrl.Length - 1)
                : baseUrl;

            services
                .ConfigureDefaultForApi<Startup>(new StartupConfigureOptions
                {
                    Cors =
                    {
                        Origins = _configuration
                            .GetSection("Cors")
                            .GetChildren()
                            .Select(c => c.Value)
                            .ToArray()
                    },
                    Server =
                    {
                        BaseUrl = baseUrlForExceptions,
                        ProblemDetailsTypeNamespaceOverride = "be.vlaanderen.basisregisters.api"
                    },
                    Swagger =
                    {
                        ApiInfo = (provider, description) => new OpenApiInfo
                        {
                            Version = _marketingVersion,
                            Title = "Basisregisters Vlaanderen API",
                            Description = GetApiLeadingText(description, Convert.ToBoolean(_configuration.GetSection("FeatureToggles")["IsFeedsVisible"])),
                            Contact = _contact,
                            License = new OpenApiLicense
                            {
                                Name = "Modellicentie Gratis Hergebruik - v1.0",
                                Url = new Uri("https://overheid.vlaanderen.be/sites/default/files/documenten/ict-egov/licenties/hergebruik/modellicentie_gratis_hergebruik_v1_0.html")
                            },
                        },

                        CustomSortFunc = SortByApiOrder.Sort,

                        Servers = new []
                        {
                            new Server(baseUrl, _configuration.GetValue<string>("BaseName"))
                        },

                        XmlCommentPaths = new []
                        {
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(MunicipalityRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PostalRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(StreetNameRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PublicServiceRegistry.Api.Backoffice.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Common.NodaHelpers).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Legacy.Identificator).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Provenance.Provenance).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.Utilities.Rfc3339SerializableDateTimeOffset).GetTypeInfo().Assembly.GetName().Name,
                        },

                        MiddlewareHooks =
                        {
                            AfterSwaggerGen = x =>
                            {
                                x.OperationFilter<RemoveParameterOperationFilter>("sort");
                                x.OperationFilter<ProblemDetailsOperationFilter>();
                            }
                        }
                    },
                    Localization =
                    {
                        DefaultCulture = new CultureInfo(DefaultCulture),
                        SupportedCultures = SupportedCultures
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => new CultureInfo(x.Trim()))
                            .ToArray()
                    },
                    MiddlewareHooks =
                    {
                        FluentValidation = fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>(),

                        AfterMvcCore = builder =>
                        {
                            builder
                                .AddMvcOptions(options =>
                                {
                                    //GRAR-1877
                                    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value,fieldName) => $"De waarde '{value}' is ongeldig voor {fieldName}.");

                                    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(value => $"Waarde ontbreekt voor de parameter '{value}'.");
                                    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => $"Verplichte invoer.");
                                    options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => $"De body van het verzoek mag niet leeg zijn.");

                                    options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"De waarde '{value}' is ongeldig.");
                                    options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => $"De opgegeven waarde is ongeldig.");
                                    options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => $"De waarde in het veld moet numeriek zijn.");

                                    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(fieldName => $"De opgegeven waarde is ongeldig voor {fieldName}.");

                                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(value => $"De waarde '{value}' is ongeldig.");
                                    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(fieldName => $"De waarde in het veld {fieldName} moet numeriek zijn.");
                                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => $"De waarde '{value}' is ongeldig.");
                                });

                            builder.ConfigureApplicationPartManager(apm =>
                            {
                                var parts = apm.ApplicationParts;
                                var unneededParts = parts.Where(part => part.Name.Contains("Registry.Api")).ToArray();

                                foreach (var unneededPart in unneededParts)
                                    parts.Remove(unneededPart);
                            });
                        },

                        AfterMvc = builder => builder.Services.Configure<ApiBehaviorOptions>(options =>
                        {
                            options.SuppressInferBindingSourcesForParameters = true;

                            options.InvalidModelStateResponseFactory = actionContext =>
                            {
                                actionContext.SetContentFormatAcceptType();
                                var httpContext = actionContext.HttpContext;
                                httpContext
                                    .Request
                                    .RewriteAcceptTypeForProblemDetail();

                                var problemDetailsHelper = httpContext
                                    .RequestServices
                                    .GetRequiredService<ProblemDetailsHelper>();

                                var modelStateProblemDetails = new ModelStateProblemDetails(actionContext.ModelState)
                                {
                                    ProblemInstanceUri = problemDetailsHelper.GetInstanceUri(httpContext)
                                };
                                modelStateProblemDetails.ProblemTypeUri = problemDetailsHelper.RewriteExceptionTypeFrom(modelStateProblemDetails);

                                return new BadRequestObjectResult(modelStateProblemDetails)
                                {
                                    ContentTypes =
                                    {
                                        AcceptTypes.JsonProblem,
                                        AcceptTypes.XmlProblem
                                    }
                                };
                            };
                        }),
                        ConfigureProblemDetails = cfg => cfg.OnBeforeWriteDetails = (context, _) =>
                        {
                            context
                                .Request
                                .RewriteAcceptTypeForProblemDetail();
                        }
                    }
                })

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
                    },
                    lastModifiedInjectorFunc: _ => new RedisLastModifiedInjector())

                .ConfigureRegistryOptions<MunicipalityOptions>(_configuration.GetSection("ApiConfiguration:MunicipalityRegistry"))
                .ConfigureRegistryOptions<PostalOptions>(_configuration.GetSection("ApiConfiguration:PostalRegistry"))
                .ConfigureRegistryOptions<StreetNameOptions>(_configuration.GetSection("ApiConfiguration:StreetNameRegistry"))
                .ConfigureRegistryOptions<AddressOptions>(_configuration.GetSection("ApiConfiguration:AddressRegistry"))
                .ConfigureRegistryOptions<BuildingOptions>(_configuration.GetSection("ApiConfiguration:BuildingRegistry"))
                .ConfigureRegistryOptions<ParcelOptions>(_configuration.GetSection("ApiConfiguration:ParcelRegistry"))
                .Configure<FeatureToggleOptions>(_configuration.GetSection("FeatureToggles"))
                .AddSingleton(c => new FeedsVisibleToggle(c.GetService<IOptions<FeatureToggleOptions>>().Value.IsFeedsVisible));

            services
                .RemoveAll<IApiControllerSpecification>()
                .TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ToggledApiControllerSpec>());

            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterModule(new ApiConfigurationModule(_configuration))
                .RegisterModule(new DataDogModule(_configuration))
                .RegisterModule(new RedisModule(_configuration))
                .RegisterModule(new ExtractDownloadModule(_configuration, _marketingVersion))
                .RegisterModule(new StatusModule(_configuration))
                .RegisterModule(new InfoModule(_configuration));

            containerBuilder.Populate(services);

            containerBuilder
                .RegisterAssemblyTypes(
                    AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .Where(x => x.FullName.Contains("Registry.Api") || x.FullName.Contains("Be.Vlaanderen.Basisregisters.Api"))
                        .ToArray())
                .AsClosedTypesOf(typeof(IExamplesProvider<>))
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsSubClassOfGeneric(typeof(RegistryApiController<>)))
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<FeedController>()
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<ExtractController>()
                .WithAttributeFiltering();

            containerBuilder
                .RegisterInstance(_marketingVersion);

            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            ApiDataDogToggle datadogToggle,
            ApiDebugDataDogToggle debugDataDogToggle)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;

            app
                .UseDataDog<Startup>(new DataDogOptions
                {
                    Common =
                    {
                        ServiceProvider = serviceProvider,
                        LoggerFactory = loggerFactory
                    },
                    Toggles =
                    {
                        Enable = datadogToggle,
                        Debug = debugDataDogToggle
                    },
                    Tracing =
                    {
                        ServiceName = _configuration["DataDog:ServiceName"],
                        TraceIdHeaderName = "X-Amzn-Trace-Id",
                        TraceIdGenerator = traceHeader =>
                        {
                            var awsTraceId = traceHeader
                                .ToString()
                                .Replace("\"", string.Empty)
                                .Replace("Root=", string.Empty);

                            var traceIdHash = Sha1.ComputeHash(Encoding.UTF8.GetBytes(awsTraceId));
                            var traceIdHex = BitConverter.ToString(traceIdHash).Replace("-", string.Empty);
                            var traceIdNumber = BigInteger.Parse(traceIdHex, NumberStyles.HexNumber);
                            var traceId = (long)BigInteger.Remainder(traceIdNumber, new BigInteger(Math.Pow(10, 14)));
                            return Math.Abs(traceId);
                        }
                    }
                })

                .UseDefaultForApi(new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer,
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory,
                    },
                    Api =
                    {
                        DefaultCorsPolicy = StartupHelpers.AllowAnyOrigin,
                        VersionProvider = apiVersionProvider,
                        Info = groupName => $"Basisregisters Vlaanderen - API {_marketingVersion}",
                        Description = _ => "Een stelsel van authentieke gegevensbronnen van de Vlaamse Overheid.",
                        ApplicationName = _ => "Basisregisters Vlaanderen",
                        HeaderTitle = groupName => "Basisregisters Vlaanderen",
                        HeaderLink = groupName => _configuration["SiteUrl"],
                        FooterVersion = $"{version.Minor}.{version.Build}.{version.Revision}",
                        HeadContent = _ => @"
                            <style>
                                input.search-input {
                                    border-bottom: 0px
                                }

                                ul[role=""navigation""] + div {
                                    display: none;
                                }

                                div[data-section-id] {
                                    padding: 15px 0px;
                                }

                                li[data-item-id=""section/Introductie""],
                                li[data-item-id=""tag/Gemeenten""],
                                li[data-item-id=""tag/CRAB-Huisnummers""]
                                {
                                    border-top: 1px solid rgb(225, 225, 225);
                                }
                            </style>",
                        CSharpClientOptions =
                        {
                            ClassName = "BaseRegistry",
                            Namespace = "Be.Vlaanderen.Basisregisters"
                        },
                        TypeScriptClientOptions =
                        {
                            ClassName = "BaseRegistry"
                        },
                        ProblemDetailsExceptionMappers = new [] { new GrbWfsExceptionMapping() }
                    },
                    MiddlewareHooks =
                    {
                        AfterResponseCompression = x => x.UseHttpCacheHeaders(),
                    }
                });
        }

        private string GetApiLeadingText(ApiVersionDescription description, bool isFeedsVisibleToggle)
        {
            var text = new StringBuilder(1000);

            text.Append(
$@"# Introductie

Welkom bij de REST API van Basisregisters Vlaanderen!

Momenteel leest u de documentatie voor versie {_marketingVersion} van de Basisregisters Vlaanderen API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund**." : ".")}

[REST](http://en.wikipedia.org/wiki/REST_API) is een webserviceprotocol dat zich leent tot snelle ontwikkeling door het gebruik van HTTP- en JSON-technologie.

Basisregisters Vlaanderen stelt u in staat om alles te weten te komen rond:
* de Belgische gemeenten;
* de Belgische postcodes;
* de Vlaamse straatnamen;
* de Vlaamse adressen;
* de Vlaamse gebouwen en gebouweenheden;
* de Vlaamse percelen;
* de Vlaamse organisaties en organen;
* de publieke dienstverleningen in Vlaanderen.

Basisregisters Vlaanderen is de authentieke bron rond al bovenstaande gegevens met uitzondering van gemeenten, postcodes en percelen, die wij aanbieden als referentiebron.

## Contact

U kan ons bereiken via [{_contact.Email}](mailto:{_contact.Email}).

# Technische Info

## Basis-URL

De REST API van Basisregisters Vlaanderen is te bereiken via volgende basis-URL.

Doelpubliek | REST basis-URL                                                    |
----------- | ----------------------------------------------------------------- |
Iedereen    | {_configuration["BaseUrl"]}{description.GroupName} |

## Toegang tot de API

U kan anoniem gebruik maken van de API, maar deze anonieme toegang is beperkt in het aantal verzoeken dat u tegelijk kan sturen.

Wenst u volwaardige toegang tot de API, [kan u hier een sleutel aanvragen](https://dynamicforms.crmiv.vlaanderen.be/DynamicForms/APIKey-aanvraag). Die gebruikt u dan voortaan door bij de API request een HTTP header genaamd `x-api-key` toe te voegen, met als waarde uw API-sleutel.

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
");

            if (isFeedsVisibleToggle)
                text.AppendLine(
                    $@"## Gebruik van feeds

### Beoogde toepassing

De endpoints onder [Feeds](#tag/Feeds) laten u toe om alle wijzigingen per objecttype of ‘resource’ op te vragen. Deze maken gebruik van [Atom](https://en.wikipedia.org/wiki/Atom_(Web_standard)) als standaard.

Aan de hand van een feed kan u de wijzigingen op drie manieren opvragen: als gebeurtenissen(‘business events’), als de daaruit resulterende objectversies, of een combinatie van beide.Dit doet u door aan de `embed` parameter respectievelijk `event`, `object` of `object,event` mee te geven.

U gebruikt de `from` parameter om een startpunt te kiezen vanaf waar u de wijzigingen wilt binnenhalen, in combinatie met de `limit` parameter voor het aantal wijzigingen.

Deze functionaliteit stelt u in staat een pull-based mechanisme te bouwen om op de hoogte te blijven van voor u relevante wijzigingen. Zo kan u uw lokale databank bijwerken met de laatst beschikbare informatie uit het centrale register, of kan u bijvoorbeeld de gebeurtenissen als trigger gebruiken om uw bedrijfsprocessen te activeren (bv.IF[‘AddressWasRetired’ AND ‘dossier gekoppeld aan adres’] THEN ‘check of dossier mag afgesloten worden’).

### Aan de slag

In pseudo-code zou u als volgt wijzigingen binnenhalen:

* Roep het feed endpoint aan van het objecttype dat u wenst, zonder `from` parameter.
* Lees het `<link>` veld met `rel=""next""` uit om de volgende pagina met wijzigingen te weten te komen.
* Lees de gevraagde gegevens uit en sla het `<id>` veld van de laatste `<entry>` op de pagina op zodat u weet tot hoever u de wijzigingen reeds verwerkt hebt.
* Roep nu de volgende pagina met wijzigingen aan.
* Herhaal dit tot u alle gegevens hebt verwerkt en er geen volgende pagina meer is.

Wanneer uw proces zou stopgezet of onderbroken worden, kan u eenvoudig terug oppikken waar u gebleven was:

* Roep opnieuw het feed endpoint aan, maar dit keer met de `from` parameter 1 groter dan het laatste `<id>` veld dat u hebt uitgelezen.
* Voer bovenstaande stappen uit om alle gegevens te verwerken.

In het veld `<content>` kan u het event en/of de objectversiedetails terugvinden per wijziging (`<entry>`).

### Betekenis van de events en velden in de feed

Een overzicht van alle mogelijke business events en de betekenis van de attributen onder het blokje `<event>` vindt u op deze pagina: [{_configuration["BaseUrl"]}{description.GroupName}/info/events?tags=sync]({_configuration["BaseUrl"]}{description.GroupName}/info/events?tags=sync).

### Kanttekening

Merk op dat de granulariteit vrij hoog is door het doorvertalen van de volledige CRAB-historiek(legacysysteem) naar het Gebouwen- en Adressenregister(GR-AR). Om dezelfde reden zult u zien dat de meeste objecten gradueel opgebouwd worden(toevoegen status, geometrie enz.) tot wanneer ze ‘complete’ zijn.

Het ‘compleet worden van een object’ (wat betekent dat het object nu over alle attributen beschikt volgens het GR-AR-informatiemodel) wordt aangegeven met een apart event.

De persistente identificator van een object (van de vorm `https://data.vlaanderen.be/id/<objecttype>/<persistentelokaleid>`) waarmee u naar het object kunt verwijzen in uw toepassingen, wordt beschikbaar vanaf het event `<objecttype>PersistentLocalIdentifierWasAssigned`.

Wanneer deze identificator nog niet beschikbaar is kunt u gebruik maken van de technische sleutel (GUID die ook in het antwoord aanwezig is) om alle events op één object aan elkaar te relateren. Deze GUID kan enkel gebruikt worden binnen de feed. Voor communicatie met derde partijen dient de persistente identificator gebruikt te worden.

Het is onze intentie om bij het opzetten van decentraal beheer op het register de granulariteit van de events te herbekijken om het gebruik van de feed in de toekomst te vereenvoudigen.

### API key verplicht

Om de [Feeds](#tag/Feeds) te gebruiken is het verplicht om een API key mee te geven. Als u dit namelijk niet doet dan krijgt u een errormelding 401 als response terug. Er zijn 2 mogelijkheden om de API key mee te geven:

* Via de header `x-api-key`.
* In de URL. Bijvoorbeeld: `{_configuration["BaseUrl"]}{description.GroupName}/feeds/adressen?apikey={{apikey}}` waarbij `{{apikey}}` vervangen wordt door de unieke code van uw API key.");

            return text.ToString();
        }
    }
}
