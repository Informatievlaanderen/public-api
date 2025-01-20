namespace Common.FeatureToggles
{
    public sealed class AttachAddressParcelToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ParcelAttachAddress";

        public AttachAddressParcelToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DetachAddressParcelToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ParcelDetachAddress";

        public DetachAddressParcelToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
