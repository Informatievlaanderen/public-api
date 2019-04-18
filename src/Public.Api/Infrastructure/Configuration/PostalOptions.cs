namespace Public.Api.Infrastructure.Configuration
{
    using PostalRegistry.Api.Legacy.Infrastructure.Options;

    public class PostalOptions : ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
