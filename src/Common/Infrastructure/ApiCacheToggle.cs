namespace Common.Infrastructure
{
    using FeatureToggle;

    public class ApiCacheToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApiCacheToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
