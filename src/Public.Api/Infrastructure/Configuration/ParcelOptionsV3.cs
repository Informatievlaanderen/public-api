namespace Public.Api.Infrastructure.Configuration
{
    using ParcelRegistry.Api.Oslo.Infrastructure.Options;

    public class ParcelOptionsV3 : ResponseOptionsV3, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
