namespace Public.Api.Infrastructure.Configuration
{
    using ParcelRegistry.Api.Oslo.Infrastructure.Options;

    public class ParcelOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
