namespace Public.Api.Infrastructure.Configuration
{
    public class FeatureToggleOptions
    {
        public const string ConfigurationKey = "FeatureToggles";
        public bool IsFeedsVisible { get; set; }
        public bool ProposeStreetName { get; set; }
    }
}
