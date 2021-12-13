namespace Public.Api.Infrastructure.Configuration
{
    using BuildingRegistry.Api.Oslo.Infrastructure.Options;

    public class BuildingOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
