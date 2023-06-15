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
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Microsoft;
    using Be.Vlaanderen.Basisregisters.DependencyInjection;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using Common.Infrastructure.Extensions;
    using Common.Infrastructure.Modules;
    using Configuration;
    using Extract;
    using Feeds;
    using Feeds.V2;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
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
    using RoadRegistry.BackOffice.Api.Infrastructure.SchemaFilters;
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
                        ApiInfo = (provider, description) => new OpenApiInfo
                        {
                            Version = _marketingVersion,
                            Title = "Basisregisters Vlaanderen API",
                            Description = GetApiLeadingText(
                                provider.ApiVersionDescriptions.FirstOrDefault(x => !x.IsDeprecated),
                                Convert.ToBoolean(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey)["IsFeedsVisible"]),
                                Convert.ToBoolean(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey)["ProposeStreetName"])),
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
                            typeof(StreetNameRegistry.Api.BackOffice.Abstractions.Requests.ProposeStreetNameRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(AddressRegistry.Api.BackOffice.Abstractions.Requests.ApproveAddressRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.Legacy.Infrastructure.Options.ResponseOptions).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.Oslo.Infrastructure.Options.ResponseOptions).GetTypeInfo().Assembly.GetName().Name,
                            typeof(BuildingRegistry.Api.BackOffice.Abstractions.Building.Responses.PlanBuildingResponse).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Legacy.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.Oslo.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(ParcelRegistry.Api.BackOffice.Abstractions.Requests.AttachAddressRequest).GetTypeInfo().Assembly.GetName().Name,
                            typeof(PublicServiceRegistry.Api.Backoffice.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(RoadRegistry.BackOffice.Api.Infrastructure.Startup).GetTypeInfo().Assembly.GetName().Name,
                            typeof(RoadRegistry.BackOffice.Abstractions.EndpointRequest).GetTypeInfo().Assembly.GetName().Name,
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

                                //TODO-rik with the next WR release this will be wrapped in a WR extension method
                                x.SchemaFilter<OutlinedRoadSegmentMorphologySchemaFilter>();
                                x.SchemaFilter<OutlinedRoadSegmentStatusSchemaFilter>();
                                x.SchemaFilter<OutlinedRoadSegmentSurfaceTypeSchemaFilter>();
                                x.SchemaFilter<RoadSegmentAccessRestrictionSchemaFilter>();
                                x.SchemaFilter<RoadSegmentCategorySchemaFilter>();
                                x.SchemaFilter<RoadSegmentGeometryDrawMethodSchemaFilter>();
                                x.SchemaFilter<RoadSegmentLaneDirectionSchemaFilter>();
                                x.SchemaFilter<RoadSegmentMorphologySchemaFilter>();
                                x.SchemaFilter<RoadSegmentStatusSchemaFilter>();
                                x.SchemaFilter<RoadSegmentSurfaceTypeSchemaFilter>();
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
                .AddSingleton(c => new RemoveStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RemoveStreetName))
                .AddSingleton(c => new CorrectStreetNameRetirementToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameRetirement))
                .AddSingleton(c => new CorrectStreetNameNamesToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameNames))
                .AddSingleton(c => new CorrectStreetNameHomonymAdditionsToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectStreetNameHomonymAdditions))
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
                .AddSingleton(c => new CorrectRegularizationAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectRegularizationAddress))
                .AddSingleton(c => new CorrectDeregulationAddressToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectDeregulationAddress))
                .AddSingleton(c => new ReaddressStreetNameAddressesToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ReaddressStreetNameAddresses))

                .AddSingleton(c => new PlanBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.PlanBuilding))
                .AddSingleton(c => new BuildingUnderConstructionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.BuildingUnderConstruction))
                .AddSingleton(c => new CorrectBuildingUnderConstructionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnderConstruction))
                .AddSingleton(c => new RealizeBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RealizeBuilding))
                .AddSingleton(c => new CorrectBuildingRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingRealization))
                .AddSingleton(c => new NotRealizeBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.NotRealizeBuilding))
                .AddSingleton(c => new CorrectBuildingNotRealizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingNotRealization))
                .AddSingleton(c => new ChangeBuildingGeometryOutlineToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeBuildingGeometryOutline))
                .AddSingleton(c => new DemolishBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DemolishBuilding))
                .AddSingleton(c => new RemoveBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RemoveBuilding))
                .AddSingleton(c => new ChangeGeometryBuilding(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeGeometryBuilding))
                .AddSingleton(c => new CorrectGeometryBuildingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectGeometryBuilding))
                .AddSingleton(c => new BuildingGrbUploadJobToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.BuildingGrbUploadJob))

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
                .AddSingleton(c => new CorrectBuildingUnitRegularizationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitRegularization))
                .AddSingleton(c => new DeregulateBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DeregulateBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitDeregulationToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitDeregulation))
                .AddSingleton(c => new ChangeFunctionBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeFunctionBuildingUnit))
                .AddSingleton(c => new CorrectFunctionBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectFunctionBuildingUnit))
                .AddSingleton(c => new CorrectBuildingUnitPositionToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CorrectBuildingUnitPosition))
                .AddSingleton(c => new RemoveBuildingUnitToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.RemoveBuildingUnit))

                .AddSingleton(c => new AttachAddressParcelToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.AttachAddressParcel))
                .AddSingleton(c => new DetachAddressParcelToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DetachAddressParcel))

                .AddSingleton(c => new TicketingToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.Ticketing))

                .AddSingleton(c => new ChangeRoadSegmentAttributesToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeRoadSegmentAttributes))
                .AddSingleton(c => new ChangeRoadSegmentOutlineGeometryToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.ChangeRoadSegmentOutlineGeometry))
                .AddSingleton(c => new CreateRoadSegmentOutlineToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.CreateRoadSegmentOutline))
                .AddSingleton(c => new DeleteRoadSegmentOutlineToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.DeleteRoadSegmentOutline))
                .AddSingleton(c => new LinkRoadSegmentStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.LinkRoadSegmentStreetName))
                .AddSingleton(c => new UnlinkRoadSegmentStreetNameToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.UnlinkRoadSegmentStreetName))
                .AddSingleton(c => new GetRoadSegmentToggle(c.GetRequiredService<IOptions<FeatureToggleOptions>>().Value.GetRoadSegment))
                ;

            services
                .RemoveAll<IApiControllerSpecification>()
                .TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ToggledApiControllerSpec>());

            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterModule(new ApiConfigurationModule(_configuration))
                .RegisterModule(new RedisModule(_configuration))
                .RegisterModule(new ExtractDownloadModule(_configuration, _marketingVersion))
                .RegisterModule(new StatusModule(_configuration))
                .RegisterModule(new InfoModule(_configuration));

            services.RegisterModule(new DataDogModule(_configuration));

            containerBuilder.Populate(services);

            RegisterExamples(containerBuilder);

            containerBuilder
                .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsSubClassOfGeneric(typeof(RegistryApiController<>)))
                .WithAttributeFiltering();

            containerBuilder
                .RegisterType<FeedController>()
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
                            new UnauthorizedExceptionMapping(),
                            new ForbiddenExceptionMapping(),
                            new ApiKeyExceptionMapping()
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

        private static bool AssemblyNameIsRegistryAssembly(string? name)
        {
            return name != null && (name.Contains("Registry.Api") || name.Contains("RoadRegistry"));
        }

        private string GetApiLeadingText(ApiVersionDescription description, bool isFeedsVisibleToggle, bool isProposeStreetName)
        {
            var text = new StringBuilder(1000);

            // Todo: below should be used once we deprecate v1.
            // var baseUrlWithGroupName = BaseUrl.Combine(description.GroupName);
            var baseUrlWithGroupName = BaseUrl.Combine("v2");
            var siteUrlWithDocs = SiteUrl.Combine("documentatie");

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

");

            if (isProposeStreetName)
                text.AppendLine(
                    $@"## Gebruik van de edit API's

### Werking edit API's
Informatie over hoe de edit API's van het gebouwen-en adressenregister werken kan gevonden worden op volgende pagina: {siteUrlWithDocs}/editendpointsgrar.
");
            return text.ToString();
        }
    }
}
