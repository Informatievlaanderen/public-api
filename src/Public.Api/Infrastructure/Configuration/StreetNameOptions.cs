namespace Public.Api.Infrastructure.Configuration
{
    using StreetNameRegistry.Api.Legacy.Infrastructure.Options;

    public class StreetNameOptions : ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
