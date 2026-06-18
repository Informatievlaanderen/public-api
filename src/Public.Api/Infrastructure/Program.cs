namespace Public.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Modules;
    using Extract;
    using Feeds.V2;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Modules;
    using Redis;
    using Swashbuckle.AspNetCore.Filters;
    using Version;

    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((hostContext, containerBuilder) =>
                {
                    var marketingVersion = new MarketingVersion(hostContext.Configuration);

                    containerBuilder
                        .RegisterModule(new ApiConfigurationModule(hostContext.Configuration))
                        .RegisterModule(new RedisModule(hostContext.Configuration))
                        .RegisterModule(new ExtractDownloadModule(hostContext.Configuration, marketingVersion))
                        .RegisterModule(new StatusModule(hostContext.Configuration))
                        .RegisterModule(new InfoModule(hostContext.Configuration));

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
                        .RegisterInstance(marketingVersion);
                })
                .UseDefaultForApi<Startup>(
                    new ProgramOptions
                    {
                        Hosting =
                        {
                            HttpPort = 2080
                        },
                        Logging =
                        {
                            WriteTextToConsole = false,
                            WriteJsonToConsole = false
                        },
                        Runtime =
                        {
                            CommandLineArgs = args
                        }
                    });

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

        private static bool AssemblyNameIsRegistryAssembly(string? name)
        {
            return name != null && (name.Contains("Registry.Api") || name.Contains("RoadRegistry") ||
                                    name.Contains("IntegrationDb"));
        }
    }
}
