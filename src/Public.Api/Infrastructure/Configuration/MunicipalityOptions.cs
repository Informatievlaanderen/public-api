namespace Public.Api.Infrastructure.Configuration
{
    using MunicipalityRegistry.Api.Legacy.Infrastructure.Options;

    public class MunicipalityOptions : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
