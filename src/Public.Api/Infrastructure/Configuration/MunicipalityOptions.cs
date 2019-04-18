namespace Public.Api.Infrastructure.Configuration
{
    using MunicipalityRegistry.Api.Legacy.Infrastructure.Options;

    public class MunicipalityOptions : ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
