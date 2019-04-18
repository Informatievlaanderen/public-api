namespace Public.Api.Infrastructure.Configuration
{
    using ParcelRegistry.Api.Legacy.Infrastructure.Options;

    public class ParcelOptions : ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
