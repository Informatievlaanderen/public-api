namespace Common.FeatureToggles
{
    public sealed class ProposeStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNamePropose";

        public ProposeStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ApproveStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameApprove";

        public ApproveStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RejectStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameReject";

        public RejectStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RetireStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameRetire";

        public RetireStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RenameStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameRename";

        public RenameStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RemoveStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameRemove";

        public RemoveStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectStreetNameRetirementToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameCorrectRetirement";

        public CorrectStreetNameRetirementToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectStreetNameNamesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameCorrectNames";

        public CorrectStreetNameNamesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectStreetNameHomonymAdditionsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameCorrectHomonymAdditions";

        public CorrectStreetNameHomonymAdditionsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectStreetNameApprovalToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameCorrectApproval";

        public CorrectStreetNameApprovalToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectStreetNameRejectionToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "StreetNameCorrectRejection";

        public CorrectStreetNameRejectionToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeFeedStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "ChangeFeedStreetNameToggle";

        public ChangeFeedStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
