namespace Common.Infrastructure
{
    using FeatureToggle;

    public class ApiRedisToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApiRedisToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
