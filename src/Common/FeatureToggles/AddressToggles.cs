namespace Common.FeatureToggles
{
    public sealed class ProposeAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressPropose";

        public ProposeAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ApproveAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressApprove";

        public ApproveAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeregulateAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressDeregulate";

        public DeregulateAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RegularizeAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressRegularize";

        public RegularizeAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RejectAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressReject";

        public RejectAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RetireAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressRetire";

        public RetireAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RemoveAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressRemove";

        public RemoveAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangePostalCodeAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressChangePostalCode";

        public ChangePostalCodeAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangePositionAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressChangePosition";

        public ChangePositionAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectHouseNumberAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectHouseNumber";

        public CorrectHouseNumberAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBoxNumberAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectBoxNumber";

        public CorrectBoxNumberAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBoxNumbersAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectBoxNumbers";

        public CorrectBoxNumbersAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectPostalCodeAddress : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectPostalCode";

        public CorrectPostalCodeAddress(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectPositionAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectPosition";

        public CorrectPositionAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectApprovalAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectApproval";

        public CorrectApprovalAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectRejectionAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectRejection";

        public CorrectRejectionAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectRetirementAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectRetirement";

        public CorrectRetirementAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectRegularizationAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectRegularization";

        public CorrectRegularizationAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectDeregulationAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectDeregulation";

        public CorrectDeregulationAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectRemovalAddressToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressCorrectRemoval";

        public CorrectRemovalAddressToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class SearchAddressesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressSearch";

        public SearchAddressesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ReaddressStreetNameAddressesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "AddressReaddressStreetName";

        public ReaddressStreetNameAddressesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            :base(dynamicFeatureToggleService)
        { }
    }
}


