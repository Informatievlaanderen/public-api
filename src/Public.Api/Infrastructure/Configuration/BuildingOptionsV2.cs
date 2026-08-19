namespace Public.Api.Infrastructure.Configuration
{
    using BuildingRegistry.Api.Oslo.Infrastructure.Options;

    public class BuildingOptionsV2 : ResponseOptionsV2, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
