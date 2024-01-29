namespace Public.Api.Infrastructure.Configuration
{
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions;

    public class SuspiciousCasesOptionsV2 : ResponseOptions, IRegistryOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
