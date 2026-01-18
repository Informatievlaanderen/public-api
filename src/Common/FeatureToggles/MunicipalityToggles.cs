namespace Common.FeatureToggles
{
    public sealed class ChangeFeedMunicipalityToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ChangeFeedMunicipalityToggle";

        public ChangeFeedMunicipalityToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
