namespace Public.Api.Infrastructure.Modules
{
    using Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling.Documentation;
    using Configuration;
    using Microsoft.Extensions.Configuration;

    public class InfoModule : Module
    {
        private readonly IConfiguration _apiConfiguration;

        public InfoModule(IConfiguration configuration)
            => _apiConfiguration = configuration.GetSection("ApiConfiguration");

        protected override void Load(ContainerBuilder builder)
        {
            RegisterEventsMarkdownGenerators(builder);
        }

        private void RegisterEventsMarkdownGenerators(ContainerBuilder builder)
        {
            void RegisterMarkDownGenerator<T>(string registryKey)
                => builder
                    .Register(container => new RegistryEventsMarkdownGenerator<T>(_apiConfiguration.GetValue<string>($"{registryKey}:EventsDocumentationHeader")))
                    .As<IRegistryEventsMarkdownGenerator>()
                    .Keyed<IRegistryEventsMarkdownGenerator>(registryKey);

            RegisterMarkDownGenerator<MunicipalityRegistry.DomainAssemblyMarker>(RegistryKeys.MunicipalityV2);
            RegisterMarkDownGenerator<PostalRegistry.DomainAssemblyMarker>(RegistryKeys.PostalV2);
            RegisterMarkDownGenerator<StreetNameRegistry.DomainAssemblyMarker>(RegistryKeys.StreetNameV2);
            RegisterMarkDownGenerator<AddressRegistry.DomainAssemblyMarker>(RegistryKeys.AddressV2);
            RegisterMarkDownGenerator<BuildingRegistry.DomainAssemblyMarker>(RegistryKeys.BuildingV2);
            RegisterMarkDownGenerator<ParcelRegistry.DomainAssemblyMarker>(RegistryKeys.ParcelV2);
            // RegisterMarkDownGenerator<RoadRegistry.BackOffice.DomainAssemblyMarker>(RegistryKeys.Road);
        }
    }
}
