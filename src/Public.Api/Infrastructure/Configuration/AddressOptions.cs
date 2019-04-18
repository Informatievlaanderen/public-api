namespace Public.Api.Infrastructure.Configuration
{
    using AddressRegistry.Api.Legacy.Infrastructure.Options;

    public class AddressOptions : ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
