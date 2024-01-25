namespace Public.Api.Infrastructure.Configuration
{
    using AddressRegistry.Api.Oslo.Infrastructure.Options;

    public class SuspiciousCasesOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
