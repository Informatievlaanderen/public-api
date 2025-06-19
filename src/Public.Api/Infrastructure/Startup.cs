namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AddressRegistry.Api.BackOffice.Abstractions.Requests;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Asp.Versioning.ApiExplorer;
    using Asp.Versioning.ApplicationModels;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.AttributeFilters;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.List;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using BuildingRegistry.Api.BackOffice.Abstractions.Building.Responses;
    using BuildingRegistry.Api.Oslo.Infrastructure.Options;
    using Common.FeatureToggles;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using Common.Infrastructure.Modules;
    using Configuration;
    using Extract;
    using Feeds.V2;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Modules;
    using ParcelRegistry.Api.BackOffice.Abstractions.Requests;
    using ProblemDetailsExceptionMappings;
    using Redis;
    using Road.Downloads;
    using RoadRegistry.BackOffice.Abstractions;
    using RoadRegistry.BackOffice.Api.Infrastructure;
    using RoadRegistry.BackOffice.Api.Infrastructure.Extensions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using Swagger;
    using Swashbuckle.AspNetCore.Filters;
    using TicketingService.Abstractions;
    using Version;
    using HttpRequestExtensions = Common.Infrastructure.Extensions.HttpRequestExtensions;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private const string DefaultCulture = "en-GB";
        private const string SupportedCultures = "en-GB;en-US;en"; //"en-GB;en-US;en;nl-BE;nl";

        private IContainer _applicationContainer;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly OpenApiContact _contact;
        private readonly MarketingVersion _marketingVersion;

        public Startup(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _webHostEnvironment = webHostEnvironment;
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

            var amazonDynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            if (_webHostEnvironment.IsDevelopment())
            {
                amazonDynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.EUWest1,
                    ServiceURL = "http://localhost:8000",
                });
            }

            var dynamoDbFeatureToggleService = new DynamoDbFeatureToggleService(amazonDynamoDbClient, _configuration["FeatureToggleTableName"]);
            dynamoDbFeatureToggleService.Initialize().GetAwaiter().GetResult();
            var keyedFeatureToggles = KeyedFeatureToggleExtensions.GetFeatureToggles(dynamoDbFeatureToggleService);
            dynamoDbFeatureToggleService.Migrate(keyedFeatureToggles).GetAwaiter().GetResult();

            services
                .ConfigureDefaultForApi<Startup>(new StartupConfigureOptions
                {
                    Cors =
                    {
                        Origins = _configuration
                            .GetSection("Cors")
                            .GetChildren()
                            .Select(c => c.Value!)
                            .ToArray(),
                        Headers = new[] {ApiKeyAuthAttribute.ApiKeyHeaderName}
                    },
                    Server =
                    {
                        BaseUrl = baseUrlForExceptions,
                        ProblemDetailsTypeNamespaceOverride = "be.vlaanderen.basisregisters.api"
                    },
                    Swagger =
                    {
                        ApiInfo = (_, _) => new OpenApiInfo
                        {
                            Version = _marketingVersion,
                            Title = "Basisregisters Vlaanderen API",
                            Description = GetApiLeadingText(),
                            Contact = _contact,
                            License = new OpenApiLicense
                            {
                                Name = "Modellicentie Gratis Hergebruik - v1.0",
                                Url = new Uri("https://overheid.vlaanderen.be/sites/default/files/documenten/ict-egov/licenties/hergebruik/modellicentie_gratis_hergebruik_v1_0.html")
                            }
                        },

                        CustomSortFunc = SortByApiOrder.Sort,

                        Servers = new []
                        {
                            new Server(baseUrl, _configuration.GetValue<string>("BaseName"))
                        },

                        XmlCommentPaths = new []
                        {
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(MunicipalityRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PostalRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(StreetNameRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ProposeStreetNameRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ApproveAddressRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ResponseOptions).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PlanBuildingResponse).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AttachAddressRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(RoadRegistry.BackOffice.Api.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(EndpointRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(SuspiciousCasesListResponse).GetTypeInfo().Assembly.GetName().Name,
                            typeof(NodaHelpers).GetTypeInfo().Assembly.GetName().Name,
                            typeof(GmlConstants).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Identificator).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Provenance).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ProblemDetails).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Rfc3339SerializableDateTimeOffset).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Ticket).GetTypeInfo().Assembly.GetName().Name
                        },

                        MiddlewareHooks =
                        {
                            AfterSwaggerGen = x =>
                            {
                                x.OperationFilter<RemoveParameterOperationFilter>("sort");
                                x.OperationFilter<ProblemDetailsOperationFilter>();
                                x.OperationFilter<XApiFilter>();
                                x.EnableAnnotations();
                                x.CustomSchemaIds(type => SwashbuckleHelpers.GetCustomSchemaId(type) ??
                                                          SwashbuckleSchemaHelper.GetSchemaId(type));

                                x.AddRoadRegistrySchemaFilters();
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
                        //FluentValidation = fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>(),
                        EnableFluentValidation = false,

                        AfterMvcCore = builder =>
                        {
                            builder
                                .AddMvcOptions(options =>
                                {
                                    //GRAR-1877
                                    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value,fieldName) => $"De waarde '{value}' is ongeldig voor {fieldName}.");

                                    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(value => $"Waarde ontbreekt voor de parameter '{value}'.");
                                    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Verplichte invoer.");
                                    options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "De body van het verzoek mag niet leeg zijn.");

                                    options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"De waarde '{value}' is ongeldig.");
                                    options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "De opgegeven waarde is ongeldig.");
                                    options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "De waarde in het veld moet numeriek zijn.");

                                    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(fieldName => $"De opgegeven waarde is ongeldig voor {fieldName}.");

                                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(value => $"De waarde '{value}' is ongeldig.");
                                    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(fieldName => $"De waarde in het veld {fieldName} moet numeriek zijn.");
                                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => $"De waarde '{value}' is ongeldig.");
                                });

                            builder.ConfigureApplicationPartManager(apm =>
                            {
                                var parts = apm.ApplicationParts;
                                var unneededParts = parts.Where(part => AssemblyNameIsRegistryAssembly(part.Name)).ToArray();

                                foreach (var unneededPart in unneededParts)
                                {
                                    parts.Remove(unneededPart);
                                }
                            });
                        },
                        AfterMvc = builder => builder.Services.Configure<ApiBehaviorOptions>(options =>
                        {
                            options.SuppressInferBindingSourcesForParameters = true;

                            options.InvalidModelStateResponseFactory = actionContext =>
                            {
                                //actionContext.SetContentFormatAcceptType(); //TODO: WHY?
                                var httpContext = actionContext.HttpContext;
                                HttpRequestExtensions.RewriteAcceptTypeForProblemDetail(httpContext
                                        .Request);

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
                            HttpRequestExtensions.RewriteAcceptTypeForProblemDetail(context
                                    .Request);
                        }
                    },
                    ActionModelConventions =
                    {
                        new ApiVisibleActionModelConvention(),
                        new FeatureToggleConvention(keyedFeatureToggles)
                    }
                }
                .EnableJsonErrorActionFilterOption())

                .AddHttpClient()

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
                        var redisProvider = x.GetRequiredService<ConnectionMultiplexerProvider>();
                        var redis = redisProvider.GetConnectionMultiplexer();
                        return new RedisStore(_loggerFactory.CreateLogger<RedisStore>(), redis);
                    },
                    lastModifiedInjectorFunc: _ => new RedisLastModifiedInjector())

                .ConfigureRegistryOptions<MunicipalityOptionsV2>(_configuration.GetSection("ApiConfiguration:MunicipalityRegistryV2"))
                .ConfigureRegistryOptions<PostalOptionsV2>(_configuration.GetSection("ApiConfiguration:PostalRegistryV2"))
                .ConfigureRegistryOptions<StreetNameOptionsV2>(_configuration.GetSection("ApiConfiguration:StreetNameRegistryV2"))
                .ConfigureRegistryOptions<AddressOptionsV2>(_configuration.GetSection("ApiConfiguration:AddressRegistryV2"))
                .ConfigureRegistryOptions<BuildingOptionsV2>(_configuration.GetSection("ApiConfiguration:BuildingRegistryV2"))
                .ConfigureRegistryOptions<ParcelOptionsV2>(_configuration.GetSection("ApiConfiguration:ParcelRegistryV2"))
                .ConfigureRegistryOptions<SuspiciousCasesOptionsV2>(_configuration.GetSection("ApiConfiguration:SuspiciousCases"))
                .Configure<ExcludedRouteModelOptions>(_configuration.GetSection("ExcludedRoutes"))
                .AddSingleton<IAmazonDynamoDB>(_ => amazonDynamoDbClient)
                .AddSingleton<IDynamicFeatureToggleService>(_ => dynamoDbFeatureToggleService)
                .RegisterFeatureToggles();

            services
                .RemoveAll<IApiControllerSpecification>();

            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterModule(new ApiConfigurationModule(_configuration))
                .RegisterModule(new RedisModule(_configuration))
                .RegisterModule(new ExtractDownloadModule(_configuration, _marketingVersion))
                .RegisterModule(new StatusModule(_configuration))
                .RegisterModule(new InfoModule(_configuration));

            containerBuilder.Populate(services);

            RegisterExamples(containerBuilder);

            containerBuilder
                .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsSubClassOfGeneric(typeof(RegistryApiController<>)))
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<FeedV2Controller>()
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<ExtractController>()
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<DownloadController>()
                .WithAttributeFiltering();

            containerBuilder
                .RegisterInstance(_marketingVersion);

            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        private static void RegisterExamples(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterAssemblyTypes(
                    AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .Where(x => AssemblyNameIsRegistryAssembly(x.FullName)
                                    // We are explicitly registering the IExamplesProvider<> types from Be.Vlaanderen.Basisregisters.Api
                                    // because some providers inherit from each other which causes the wrong implementation to be resolved,
                                    // e.g. BadRequestResponseExamples as BadRequestResponseExamplesV2
                                    // || (x.FullName ?? string.Empty).Contains("Be.Vlaanderen.Basisregisters.Api")
                                    )
                        .ToArray())
                .AsClosedTypesOf(typeof(IExamplesProvider<>))
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<NotModifiedResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<BadRequestResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<BadRequestResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<ConflictResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<ConflictResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<ForbiddenResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<ForbiddenResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<InternalServerErrorResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<InternalServerErrorResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<NotAcceptableResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<PreconditionFailedResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<PreconditionFailedResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<TooManyRequestsResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<TooManyRequestsResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<UnauthorizedResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<UnauthorizedResponseExamplesV2>()
                .AsImplementedInterfaces()
                .AsSelf();

            containerBuilder
                .RegisterType<ValidationErrorResponseExamples>()
                .AsImplementedInterfaces()
                .AsSelf();
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;

            app
                .UseDefaultForApi(new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer,
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory
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
                        FooterVersion = $"{version?.Major}.{version?.Minor}.{version?.Build}",
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
                        ProblemDetailsExceptionMappers = new List<ApiProblemDetailsExceptionMapping>
                        {
                            new GoneExceptionMapping(),
                            new NotFoundExceptionMapping(),
                            new PreconditionFailedExceptionMapping(),
                            new ConflictExceptionMapping(),
                            new UnauthorizedExceptionMapping(),
                            new ForbiddenExceptionMapping(),
                            new ApiKeyExceptionMapping()
                        }
                    },
                    MiddlewareHooks =
                    {
                        AfterApiExceptionHandler = x => x.UseMiddleware<OffsetValidationMiddleware>(),
                        AfterResponseCompression = x => x.UseConditionalHttpCacheHeaders()
                    }
                })

                .UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string> { [".jsonld"] = "application/ld+json" }),
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "context")),
                    RequestPath = "/context"
                });
        }

        private static bool AssemblyNameIsRegistryAssembly(string? name)
        {
            return name != null && (name.Contains("Registry.Api") || name.Contains("RoadRegistry") || name.Contains("IntegrationDb"));
        }

        private string GetApiLeadingText()
        {
            var text = new StringBuilder(1000);

text.Append(
$@"# Introductie
Dit is de documentatiepagina van de API endpoints van het gebouwen- en adressenregister & het wegenregister.

## Contact
U kan ons bereiken via [{_contact.Email}](mailto:{_contact.Email}).

# Technische Info
## Basis-URL
De REST API van Basisregisters Vlaanderen is te bereiken via volgende basis-URL.

Doelpubliek | REST basis-URL                                                    |
----------- | ----------------------------------------------------------------- |
Iedereen    | {_configuration["BaseUrl"]} |

## API-Keys
Informatie rond het aanvragen en gebruik van API-keys kan u vinden in de [API-Keys documentatie](https://basisregisters.vlaanderen.be/apikey).

### Limieten
Elke API-key heeft een limiet van een aantal requests per seconde.
De limieten kunt u [hier](https://basisregisters.vlaanderen.be/apikey/limieten) terugvinden.

### 429 Too Many Requests

Deze API hanteert rate limiting op basis van het aantal toegestane requests per seconde per API key. Wanneer deze limiet wordt overschreden, retourneert de API een HTTP 429 Too Many Requests response.
Clients wordt aangeraden om bij ontvangst van een 429-response kort te pauzeren (bijvoorbeeld enkele honderden milliseconden) en vervolgens opnieuw te proberen.

#### Advies aan clientontwikkelaars
Om storingen te voorkomen:
 - Implementeer exponential backoff of een vaste wachttijd voordat u de request opnieuw verstuurt.
 - Zorg ervoor dat het totaal aantal requests per seconde per API key onder de limiet blijft.

");
return text.ToString();
        }
    }
}
