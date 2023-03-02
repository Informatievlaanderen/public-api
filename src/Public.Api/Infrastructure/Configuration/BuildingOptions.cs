namespace Public.Api.Infrastructure.Configuration
{
    using BuildingRegistry.Api.Legacy.Infrastructure.Options;

    public class BuildingOptions : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
