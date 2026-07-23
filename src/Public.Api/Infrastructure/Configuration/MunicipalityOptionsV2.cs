namespace Public.Api.Infrastructure.Configuration
{
    using MunicipalityRegistry.Api.Oslo.Infrastructure.Options;

    public class MunicipalityOptionsV2 : ResponseOptionsV2, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
