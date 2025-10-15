namespace Public.Api.Infrastructure.Configuration
{
    using Basisregisters.IntegrationDb.Api.Abstractions.SuspiciousCase;

    public class SuspiciousCasesOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
