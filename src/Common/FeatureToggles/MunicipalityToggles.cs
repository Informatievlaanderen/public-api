namespace Common.FeatureToggles
{
    public sealed class ChangeFeedMunicipalityToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ChangeFeedMunicipalityToggle";

        public ChangeFeedMunicipalityToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        {
        }
    }

    public sealed class OsloV3MunicipalityToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "OsloV3MunicipalityToggle";

        public OsloV3MunicipalityToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        {
        }
    }
}
