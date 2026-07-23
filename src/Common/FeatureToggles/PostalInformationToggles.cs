namespace Common.FeatureToggles
{
    public sealed class ChangeFeedPostalInformationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ChangeFeedPostalInformationToggle";

        public ChangeFeedPostalInformationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class OsloV3PostalInformationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "OsloV3PostalInformationToggle";

        public OsloV3PostalInformationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        {
        }
    }
}
