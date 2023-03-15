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

            RegisterMarkDownGenerator<MunicipalityRegistry.DomainAssemblyMarker>(RegistryKeys.Municipality);
            RegisterMarkDownGenerator<PostalRegistry.DomainAssemblyMarker>(RegistryKeys.Postal);
            RegisterMarkDownGenerator<StreetNameRegistry.DomainAssemblyMarker>(RegistryKeys.StreetName);
            RegisterMarkDownGenerator<AddressRegistry.DomainAssemblyMarker>(RegistryKeys.Address);
            RegisterMarkDownGenerator<BuildingRegistry.DomainAssemblyMarker>(RegistryKeys.Building);
            RegisterMarkDownGenerator<ParcelRegistry.DomainAssemblyMarker>(RegistryKeys.Parcel);
            RegisterMarkDownGenerator<RoadRegistry.BackOffice.DomainAssemblyMarker>(RegistryKeys.Road);
        }
    }
}
