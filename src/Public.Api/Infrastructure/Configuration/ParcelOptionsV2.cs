namespace Public.Api.Infrastructure.Configuration
{
    using ParcelRegistry.Api.Oslo.Infrastructure.Options;

    public class ParcelOptionsV2 : ResponseOptionsV2, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
