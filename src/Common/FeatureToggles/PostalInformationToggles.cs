namespace Common.FeatureToggles
{
    public sealed class ChangeFeedPostalInformationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ChangeFeedPostalInformationToggle";

        public ChangeFeedPostalInformationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
