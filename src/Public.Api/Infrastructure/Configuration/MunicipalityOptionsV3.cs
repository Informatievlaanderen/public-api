namespace Public.Api.Infrastructure.Configuration
{
    using MunicipalityRegistry.Api.Oslo.Infrastructure.Options;

    public class MunicipalityOptionsV3 : ResponseOptionsV3, IRegistryOptions
    {
        public SyndicationOptions? Syndication { get; set; }
    }
}
