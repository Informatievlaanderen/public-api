namespace Common.FeatureToggles
{
    public sealed class FeedPositionsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "FeedPositionsToggle";

        public FeedPositionsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        {
        }
    }
}
