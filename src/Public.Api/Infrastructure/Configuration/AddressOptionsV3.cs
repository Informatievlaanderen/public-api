namespace Public.Api.Infrastructure.Configuration
{
    using AddressRegistry.Api.Oslo.Infrastructure.Options;

    public class AddressOptionsV3 : ResponseOptionsV3, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
