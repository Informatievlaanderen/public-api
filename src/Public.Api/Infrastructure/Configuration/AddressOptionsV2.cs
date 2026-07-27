namespace Public.Api.Infrastructure.Configuration
{
    using AddressRegistry.Api.Oslo.Infrastructure.Options;

    public class AddressOptionsV2 : ResponseOptionsV2, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
