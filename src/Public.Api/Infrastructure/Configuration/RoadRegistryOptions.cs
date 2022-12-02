namespace Public.Api.Infrastructure.Configuration
{
    //using RoadRegistryOptions.Backoffice.Infrastructure.Options;

    public class RoadRegistryOptions : IRegistryOptions //ResponseOptions
    {
        public SyndicationOptions Syndication { get; set; }
    }
}
