namespace Public.Api.Infrastructure.Configuration
{
    using PostalRegistry.Api.Oslo.Infrastructure.Options;

    public class PostalOptionsV3 : ResponseOptionsV3, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
