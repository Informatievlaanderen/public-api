namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
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
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Extensions;
    using Common.Infrastructure.Modules;
    using Configuration;
    using Extract;
    using Feeds;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Modules;
    using ProblemDetailsExceptionMapping;
    using ProblemDetailsExceptionMappings;
    using Redis;
    using Road.Downloads;
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

        private Url BaseUrl => new Url(_configuration["BaseUrl"]);
        private Url SiteUrl => new Url(_configuration["SiteUrl"]);

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
                            .ToArray(),
                        Headers = new[] {"x-api-key"}
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
                            Description = GetApiLeadingText(description, Convert.ToBoolean(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey)["IsFeedsVisible"]),Convert.ToBoolean(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey)["ProposeStreetName"])),
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
                            typeof(MunicipalityRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(MunicipalityRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PostalRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PostalRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(StreetNameRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(StreetNameRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(StreetNameRegistry.Api.BackOffice.Abstractions.Requests.StreetNameProposeRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.BackOffice.Abstractions.Requests.AddressApproveRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.Legacy.Abstractions.Infrastructure.Options.ResponseOptions).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.Oslo.Abstractions.Infrastructure.Options.ResponseOptions).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.BackOffice.Abstractions.Building.Responses.PlanBuildingResponse).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PublicServiceRegistry.Api.Backoffice.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Common.NodaHelpers).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Edit.GmlConstants).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Legacy.Identificator).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.GrAr.Provenance.Provenance).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails).GetTypeInfo().Assembly.GetName().Name,
                            typeof(Be.Vlaanderen.Basisregisters.Utilities.Rfc3339SerializableDateTimeOffset).GetTypeInfo().Assembly.GetName().Name,
                            typeof(TicketingService.Abstractions.Ticket).GetTypeInfo().Assembly.GetName().Name
                        },

                        MiddlewareHooks =
                        {
                            AfterSwaggerGen = x =>
                            {
                                x.OperationFilter<RemoveParameterOperationFilter>("sort");
                                x.OperationFilter<ProblemDetailsOperationFilter>();
                                x.OperationFilter<XApiFilter>();
                                x.EnableAnnotations();
                                x.CustomSchemaIds(type => SwashbuckleSchemaHelper.GetSchemaId(type));
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
                                    options.Conventions.Add(new FeatureToggleConvention(_configuration));
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
                                var unneededParts = parts.Where(part => part.Name.Contains("Registry.Api")).ToArray();

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
                    },
                    ActionModelConventions = { new ApiDocumentationHiddenConvention() }
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

                .ConfigureRegistryOptions<MunicipalityOptions>(_configuration.GetSection("ApiConfiguration:MunicipalityRegistry"))
                .ConfigureRegistryOptions<MunicipalityOptionsV2>(_configuration.GetSection("ApiConfiguration:MunicipalityRegistryV2"))
                .ConfigureRegistryOptions<PostalOptions>(_configuration.GetSection("ApiConfiguration:PostalRegistry"))
                .ConfigureRegistryOptions<PostalOptionsV2>(_configuration.GetSection("ApiConfiguration:PostalRegistryV2"))
                .ConfigureRegistryOptions<StreetNameOptions>(_configuration.GetSection("ApiConfiguration:StreetNameRegistry"))
                .ConfigureRegistryOptions<StreetNameOptionsV2>(_configuration.GetSection("ApiConfiguration:StreetNameRegistryV2"))
                .ConfigureRegistryOptions<AddressOptions>(_configuration.GetSection("ApiConfiguration:AddressRegistry"))
                .ConfigureRegistryOptions<AddressOptionsV2>(_configuration.GetSection("ApiConfiguration:AddressRegistryV2"))
                .ConfigureRegistryOptions<BuildingOptions>(_configuration.GetSection("ApiConfiguration:BuildingRegistry"))
                .ConfigureRegistryOptions<BuildingOptionsV2>(_configuration.GetSection("ApiConfiguration:BuildingRegistryV2"))
                .ConfigureRegistryOptions<ParcelOptions>(_configuration.GetSection("ApiConfiguration:ParcelRegistry"))
                .ConfigureRegistryOptions<ParcelOptionsV2>(_configuration.GetSection("ApiConfiguration:ParcelRegistryV2"))
                .Configure<FeatureToggleOptions>(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey))
                .Configure<ExcludedRouteModelOptions>(_configuration.GetSection("ExcludedRoutes"))
                .AddSingleton(c => new FeedsVisibleToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsFeedsVisible))
                .AddSingleton(c => new ProposeStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ProposeStreetName))
                .AddSingleton(c => new ApproveStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ApproveStreetName))
                .AddSingleton(c => new RejectStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RejectStreetName))
                .AddSingleton(c => new RetireStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RetireStreetName))
                .AddSingleton(c => new CorrectStreetNameRetirementToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameRetirement))
                .AddSingleton(c => new CorrectStreetNameNamesToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameNames))
                .AddSingleton(c => new CorrectStreetNameApprovalToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameApproval))
                .AddSingleton(c => new CorrectStreetNameRejectionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameRejection))
                .AddSingleton(c => new ProposeAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ProposeAddress))
                .AddSingleton(c => new ApproveAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ApproveAddress))
                .AddSingleton(c => new DeregulateAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DeregulateAddress))
                .AddSingleton(c => new RegularizeAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RegularizeAddress))
                .AddSingleton(c => new RejectAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RejectAddress))
                .AddSingleton(c => new RetireAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RetireAddress))
                .AddSingleton(c => new RemoveAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RemoveAddress))
                .AddSingleton(c => new ChangePostalCodeAddress(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangePostalCodeAddress))
                .AddSingleton(c => new ChangePositionAddress(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangePositionAddress))
                .AddSingleton(c => new CorrectHouseNumberAddress(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectHouseNumberAddress))
                .AddSingleton(c => new CorrectBoxNumberAddress(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBoxNumberAddress))
                .AddSingleton(c => new CorrectPostalCodeAddress(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectPostalCodeAddress))
                .AddSingleton(c => new CorrectPositionAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectPositionAddress))
                .AddSingleton(c => new CorrectApprovalAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectApprovalAddress))
                .AddSingleton(c => new CorrectRejectionAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectRejectionAddress))
                .AddSingleton(c => new CorrectRetirementAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectRetirementAddress))

                .AddSingleton(c => new PlanBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.PlanBuilding))
                .AddSingleton(c => new BuildingUnderConstructionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.BuildingUnderConstruction))
                .AddSingleton(c => new CorrectBuildingUnderConstructionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnderConstruction))
                .AddSingleton(c => new RealizeBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RealizeBuilding))
                .AddSingleton(c => new CorrectBuildingRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingRealization))
                .AddSingleton(c => new NotRealizeBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.NotRealizeBuilding))
                .AddSingleton(c => new CorrectBuildingNotRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingNotRealization))
                .AddSingleton(c => new ChangeBuildingGeometryOutlineToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeBuildingGeometryOutline))
                .AddSingleton(c => new DemolishBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DemolishBuilding))
                .AddSingleton(c => new ChangeGeometryBuilding(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeGeometryBuilding))
                .AddSingleton(c => new CorrectGeometryBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectGeometryBuilding))

                .AddSingleton(c => new PlanBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.PlanBuildingUnit))
                .AddSingleton(c => new RealizeBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RealizeBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitRealization))
                .AddSingleton(c => new NotRealizeBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.NotRealizeBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitNotRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitNotRealization))
                .AddSingleton(c => new RetireBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RetireBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitRetirementToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitRetirement))
                .AddSingleton(c => new AttachAddressBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.AttachAddressBuildingUnit))
                .AddSingleton(c => new DetachAddressBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DetachAddressBuildingUnit))
                .AddSingleton(c => new RegularizeBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RegularizeBuildingUnit))
                .AddSingleton(c => new DeregulateBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DeregulateBuildingUnit))
                .AddSingleton(c => new ChangeBuildingUnitFunctionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeBuildingUnitFunction))
                .AddSingleton(c => new CorrectFunctionBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectFunctionBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitPositionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitPosition))
                .AddSingleton(c => new RemoveBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RemoveBuildingUnit))

                .AddSingleton(c => new AttachAddressParcelToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.AttachAddressParcel))
                .AddSingleton(c => new DetachAddressParcelToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DetachAddressParcel))

                .AddSingleton(c => new IsAddressOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsAddressOsloApiEnabled))
                .AddSingleton(c => new IsBuildingOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsBuildingOsloApiEnabled))
                .AddSingleton(c => new IsBuildingUnitOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsBuildingUnitOsloApiEnabled))
                .AddSingleton(c => new IsMunicipalityOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsMunicipalityOsloApiEnabled))
                .AddSingleton(c => new IsParcelOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsParcelOsloApiEnabled))
                .AddSingleton(c => new IsPostalCodeOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsPostalCodeOsloApiEnabled))
                .AddSingleton(c => new IsStreetNameOsloApiEnabledToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.IsStreetNameOsloApiEnabled))

                .AddSingleton(c => new TicketingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.Ticketing));

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
                        .Where(x => (x.FullName ?? string.Empty).Contains("Registry.Api") || (x.FullName ?? string.Empty).Contains("Be.Vlaanderen.Basisregisters.Api"))
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
                .RegisterType<DownloadController>()
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
            var version = Assembly.GetEntryAssembly()?.GetName().Version;

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
                        FooterVersion = $"{version?.Minor}.{version?.Build}.{version?.Revision}",
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
                            new GrbWfsExceptionMapping(),
                            new GoneExceptionMapping(),
                            new NotFoundExceptionMapping(),
                            new PreconditionFailedExceptionMapping(),
                            new ConflictExceptionMapping(),
                        }
                    },
                    MiddlewareHooks =
                    {
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

        private string GetApiLeadingText(ApiVersionDescription description, bool isFeedsVisibleToggle, bool isProposeStreetName)
        {
            var text = new StringBuilder(1000);

            var baseUrlWithGroupName = BaseUrl.Combine(description.GroupName);
            var siteUrlWithDocs = SiteUrl.Combine("documentatie");

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

Basisregisters Vlaanderen is de authentieke bron rond al de bovenstaande gegevens met uitzondering van gemeenten, postcodes en percelen, die wij aanbieden als referentiebron.

## Contact

U kan ons bereiken via [{_contact.Email}](mailto:{_contact.Email}).

# Technische Info

## Basis-URL

De REST API van Basisregisters Vlaanderen is te bereiken via volgende basis-URL.

Doelpubliek | REST basis-URL                                                    |
----------- | ----------------------------------------------------------------- |
Iedereen    | {_configuration["BaseUrl"]} |

## Gebruik van de read API’s

### Toegang

De read endpoints zijn anoniem raadpleegbaar echter is er een beperking aanwezig op het aantal verzoeken dat u tegelijkertijd kan versturen naar deze endpoints. Wanneer u een API key meegeeft dan zal u meer requests kunnen versturen.

Om dus optimaal gebruik te maken van de endpoints vraagt u best een API key aan. Dit kan door uw gegevens in volgende link achter te laten: [Vraag hier uw API key aan](https://dynamicforms.crmiv.vlaanderen.be/DynamicForms/Page/Show/CfDJ8M4Eu9v84l9JmW3p7WGylS-u2ToCLC5KvqQZmZ4G99X5TBULO4n0LCDpm7870eDUOk90hogqVcE7BCVQf2u_4WlsZ7B8friBrkyuAqmXYpIX_BzvQVVo8eUZyNd-njc33Y-Z-B87y03Y2Jgukp2AN5U93jT1Xv2l0afgvenLD9k0fasSMJkt4uNzKmlr_gILGrOy%2FJSqnRom_MLu0h7sALJ8uNvPywCMsZ1zy5Lal4h63?path=APIKey-aanvraag). U kan deze API key op 2 manieren meegeven:

* Via de header x-api-key.
* In de URL. Bijvoorbeeld: {baseUrlWithGroupName}/adressen?apikey={{apikey}} waarbij {{apikey}} vervangen wordt door de unieke code van uw API key.

### V1 vs v2

Voor de read endpoints zijn er zowel v1 als v2 endpoints beschikbaar. De v2 read endpoints zijn een vernieuwde versie van de v1 read endpoints en zijn conform aan het OSLO-model. Om duidelijk aan te geven of het een v1 of een v2 endpoint is, hebben we achteraan de titels gewerkt met (v1) voor versie 1 en (v2) voor versie 2.

Wat is het verschil tussen de v1 en de v2 endpoints?

* Het content-type van v2 is ‘application/ld+json’. Van v1 is dit default ‘application+json’, maar ‘application/xml’ is ook mogelijk.
* Er zijn 2 velden bijgekomen, namelijk `@context` en `@type`.
 * Het `@context` veld bevat de linked-data context van het endpoint. Dit is een URI naar de JSON-LD file.
 * Het `@type` veld bevat het linked-data type van het endpoint.
* De geometrievelden bij ‘Vraag een adres op (v2)’, ‘Vraag een gebouw op (v2)’ en ‘Vraag een gebouweenheid op (v2)’ zijn gewijzigd. De coördinaten van het object staan vanaf nu in het gml-formaat en alle velden die met geometrie te maken hebben zijn samengevoegd onder 1 veld.

Wat betekent 'conform aan het OSLO-model'?

* Door informatie conform aan het OSLO-model te ontsluiten, kan deze vlot gecombineerd worden met datasets op het wereldwijde web. Contextuele informatie wordt aan de response van de endpoints gekoppeld waardoor ze geschikt zijn om te gebruiken in Linked Data toepassingen.
* Meer informatie over OSLO kan u hier vinden: https://overheid.vlaanderen.be/producten-diensten/oslo.

Hoe v2 endpoints visueel in browser tonen?

In de browser moet een accept header meegegeven worden bij de request. In Chrome is dit door middel van een extensie. Een voorbeeld hiervan is ‘NoRefer’. In het witte scherm dat verschijnt na het klikken op het extensie icoon moet het volgende meegegeven worden: _'accept: application/ld+json'_. Daarna wordt de pagina best opnieuw geladen.

");

            if (isFeedsVisibleToggle)
            {
                text.AppendLine(
                    $@"## Gebruik van de feed API's

### Beoogde toepassing

De endpoints onder [Feeds](#tag/Feeds) laten u toe om alle wijzigingen per objecttype of ‘resource’ op te vragen. Deze maken gebruik van [Atom](https://en.wikipedia.org/wiki/Atom_(Web_standard)) als standaard.

Aan de hand van een feed kan u de wijzigingen op drie manieren opvragen: als gebeurtenissen(‘business events’), als de daaruit resulterende objectversies, of een combinatie van beide. Dit doet u door aan de `embed` parameter respectievelijk `event`, `object` of `object,event` mee te geven.

U gebruikt de `from` parameter om een startpunt te kiezen vanaf waar u de wijzigingen wilt binnenhalen, in combinatie met de `limit` parameter voor het aantal wijzigingen.

Deze functionaliteit stelt u in staat een pull-based mechanisme te bouwen om op de hoogte te blijven van voor u relevante wijzigingen. Zo kan u uw lokale databank bijwerken met de laatst beschikbare informatie uit het centrale register, of kan u bijvoorbeeld de gebeurtenissen als trigger gebruiken om uw bedrijfsprocessen te activeren (bv. IF[‘AddressWasRetired’ AND ‘dossier gekoppeld aan adres’] THEN ‘check of dossier mag afgesloten worden’).

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

Een overzicht van alle mogelijke business events en de betekenis van de attributen onder het blokje `<event>` vindt u op deze pagina: [{baseUrlWithGroupName}/info/events?tags=sync]({baseUrlWithGroupName}/info/events?tags=sync).

### Kanttekening

Merk op dat de granulariteit vrij hoog is door het doorvertalen van de volledige CRAB-historiek(legacysysteem) naar het Gebouwen- en Adressenregister(GR-AR). Om dezelfde reden zult u zien dat de meeste objecten gradueel opgebouwd worden(toevoegen status, geometrie enz.) tot wanneer ze ‘complete’ zijn.

Het ‘compleet worden van een object’ (wat betekent dat het object nu over alle attributen beschikt volgens het GR-AR-informatiemodel) wordt aangegeven met een apart event.

De persistente identificator van een object (van de vorm `https://data.vlaanderen.be/id/<objecttype>/<persistentelokaleid>`) waarmee u naar het object kunt verwijzen in uw toepassingen, wordt beschikbaar vanaf het event `<objecttype>PersistentLocalIdentifierWasAssigned`.

Wanneer deze identificator nog niet beschikbaar is kunt u gebruik maken van de technische sleutel (GUID die ook in het antwoord aanwezig is) om alle events op één object aan elkaar te relateren. Deze GUID kan enkel gebruikt worden binnen de feed. Voor communicatie met derde partijen dient de persistente identificator gebruikt te worden.

Het is onze intentie om bij het opzetten van decentraal beheer op het register de granulariteit van de events te herbekijken om het gebruik van de feed in de toekomst te vereenvoudigen.

### Interne events

In de feed endpoints kan u alle eventids terugvinden van alle aangeboden objecttypes. Echter zal u merken dat er soms eventids niet aanwezig zijn. De eventids die niet getoond worden, zijn interne events en niet beschikbaar voor de externe gebruikers. Wanneer u een eventid van een intern event meegeeft in de URL dan zal automatisch het eerstvolgende extern eventid na het meegegeven eventid in de response getoond worden.

### API key verplicht

Om de [Feeds](#tag/Feeds) te gebruiken is het verplicht om een API key mee te geven. Als u dit namelijk niet doet dan krijgt u een errormelding 401 als response terug. Er zijn 2 mogelijkheden om de API key mee te geven:

* Via de header `x-api-key`.
* In de URL. Bijvoorbeeld: `{baseUrlWithGroupName}/feeds/adressen?apikey={{apikey}}` waarbij `{{apikey}}` vervangen wordt door de unieke code van uw API key.

[Hier](https://dynamicforms.crmiv.vlaanderen.be/DynamicForms/Page/Show/CfDJ8M4Eu9v84l9JmW3p7WGylS9LgRV8RaaFB4kfHpofS_AGLb0p5kC-wMqGmDl7zdiZ6pivD2a80ArIuYssObUVzrWbiJdBqRAf5aS3fW6cOW7ftrxjowRj90ZPyww2LzVL-25O4o1MZ3ft6Pt4qEhIjzBfD8K39e6HhNKlMt6eh-OM2G4ysteDWGVbXlwiQIfOEZHr%2FuthUZbxKimbTCrg6nToraIYmIeQQviqmNgAoyOVV?path=APIKey-aanvraag) kan u een API key aanvragen.

### Provenance

In het veld `Provenance` staat de metadata van een event. Het bestaat uit 3 onderdelen:

* `Timestamp`/`Tijdstip`: In dit veld staat het tijdstip waarop het event is uitgevoerd.
* `Organisation`/`Organisatie`: In dit veld staat de organisatie die de agent vertegenwoordigt bij het uitvoeren van een specifieke activiteit en waarvan hij/zij de vereiste autoriteit/verantwoordelijkheid heeft gekregen om dit te doen. De mogelijke waarden bij onderdeel `Event` zijn: Unknown, Municipality, NationalRegister, Akred, TeleAtlas, Vlm, Agiv, Aiv, DigitaalVlaanderen, Ngi, DePost, NavTeq, Vkbo of Other. De mogelijke waarden bij onderdeel `Object` zijn: Onbekend, Gemeente, Federale Overheidsdienst Binnenlandse Zaken (Rijksregister), Federale Overheidsdienst Financiën (Algemene Administratie van de Patrimoniumdocumentatie), TeleAtlas, Vlaamse Landmaatschappij, Agentschap voor Geografische Informatie Vlaanderen, Agentschap Informatie Vlaanderen, Digitaal Vlaanderen, Nationaal Geografisch Instituut, bpost, NavTeq, Coördinatiecel Vlaams e-government of Andere.
* `Reason`/`Reden`: In dit veld staat de aanleiding of motivatie voor de activiteit op de entiteit.

### Timestamps

De feed bevat een aantal velden waarin een timestamp staat. Hieronder staat de betekenis van de verschillende timestamps.

* `<Feed> <Updated>` : Tijdstip waarop de data feed het laatst gewijzigd werd.
* `<Entry> <Updated>` : Tijdstip waarop het event zich voordeed.
* `<Entry> <Published>` : Tijdstip waarop de eerste versie van het object aangeboden werd.

");
            }

            if (isProposeStreetName)
                text.AppendLine(
                    $@"## Gebruik van de edit API's

### Werking edit API's
Informatie over hoe de edit API's van het gebouwen-en adressenregister werken kan gevonden worden op volgende pagina: {siteUrlWithDocs}/editendpointsgrar.
");

text.AppendLine(@"
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

### Mogelijke foutmeldingen

Binnen de aangeboden endpoints zijn er een aantal foutmeldingen die kunnen voorkomen. U moet naar het veld ‘Detail’ kijken voor meer informatie.

Foutmelding | Wanneer                                                           |
----------- | ----------------------------------------------------------------- |
304    | Wanneer de request niet gewijzigd is tegenover de vorige opvraging.  |
400    | Wanneer uw verzoek foutieve data bevat. Bijvoorbeeld:        <br>     -	Wanneer het veld numeriek is, maar er geen numerieke waarde wordt meegegeven,<br>            -	Wanneer bij de request parameter een . wordt meegegeven,<br>            -	Wanneer bij endpoint ‘Crabgebouwen’ er geen parameters worden meegegeven.|
401    |Wanneer er geen API key in de feed wordt meegegeven. |
403    |Wanneer het formaat in de URL wordt meegegeven. <br> Wanneer u een API key meegeeft die niet correct is. |
404    |Wanneer het objectid niet gevonden kan worden. |
406    |Wanneer het verkeerde formaat wordt meegegeven in de accept header.|
410    |Wanneer het objectid verwijderd is.|
500    |Wanneer de response groter is dan 10MB.<br> Wanneer er een interne fout is gebeurd. <br> Wanneer de GRB WFS-service niet kan gecontacteerd worden. |
");

            return text.ToString();
        }
    }
}
