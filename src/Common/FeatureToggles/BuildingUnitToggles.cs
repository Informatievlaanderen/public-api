namespace Common.FeatureToggles
{
    public sealed class PlanBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitPlan";

        public PlanBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RealizeBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitRealize";

        public RealizeBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitRealizationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectUnitRealize";

        public CorrectBuildingUnitRealizationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class NotRealizeBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitNotRealize";

        public NotRealizeBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitNotRealizationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectUnitNotRealize";

        public CorrectBuildingUnitNotRealizationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RetireBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitRetire";

        public RetireBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitRetirementToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectRetire";

        public CorrectBuildingUnitRetirementToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitRemovalToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectRemoval";

        public CorrectBuildingUnitRemovalToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitPositionToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectPosition";

        public CorrectBuildingUnitPositionToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RemoveBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitRemove";

        public RemoveBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class AttachAddressBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitAttachAddress";

        public AttachAddressBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DetachAddressBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitDetachAddress";

        public DetachAddressBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RegularizeBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitRegularize";

        public RegularizeBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitRegularizationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectRegularize";

        public CorrectBuildingUnitRegularizationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeregulateBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitDeregulate";

        public DeregulateBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnitDeregulationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectDeregulate";

        public CorrectBuildingUnitDeregulationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeFunctionBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitChangeFunction";

        public ChangeFunctionBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectFunctionBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitCorrectFunction";

        public CorrectFunctionBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class MoveBuildingUnitToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnitMove";

        public MoveBuildingUnitToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
         : base(dynamicFeatureToggleService)
        { }
    }
}
