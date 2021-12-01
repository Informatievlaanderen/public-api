namespace Public.Api.Infrastructure.Configuration
{
    using MunicipalityRegistry.Api.Legacy.Infrastructure.Options;

    public class MunicipalityOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
