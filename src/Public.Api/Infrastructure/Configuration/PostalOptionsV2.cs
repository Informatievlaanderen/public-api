namespace Public.Api.Infrastructure.Configuration
{
    using PostalRegistry.Api.Oslo.Infrastructure.Options;

    public class PostalOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
