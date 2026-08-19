namespace Public.Api.Infrastructure.Configuration
{
    using BuildingRegistry.Api.Oslo.Infrastructure.Options;

    public class BuildingOptionsV3 : ResponseOptionsV3, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
