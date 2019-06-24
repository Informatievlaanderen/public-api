namespace Public.Api.Infrastructure.Configuration
{
    using AddressRegistry.Api.Legacy.Infrastructure.Options;

    public class AddressOptions : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
