namespace Common.FeatureToggles
{
    public sealed class PlanBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingPlan";

        public PlanBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class MergeBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingMerge";

        public MergeBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class BuildingUnderConstructionToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingUnderConstruction";

        public BuildingUnderConstructionToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingUnderConstructionToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectUnderConstruction";

        public CorrectBuildingUnderConstructionToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RealizeBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingRealize";

        public RealizeBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingRealizationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectRealization";

        public CorrectBuildingRealizationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class NotRealizeBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingNotRealize";

        public NotRealizeBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectBuildingNotRealizationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectNotRealization";

        public CorrectBuildingNotRealizationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeBuildingGeometryOutlineToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingChangeGeometryOutline";

        public ChangeBuildingGeometryOutlineToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RemoveBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingRemove";

        public RemoveBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DemolishBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingDemolish";

        public DemolishBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeGeometryBuilding : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingChangeGeometry";

        public ChangeGeometryBuilding(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CorrectGeometryBuildingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingCorrectGeometry";

        public CorrectGeometryBuildingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class BuildingGrbUploadJobToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "BuildingGrbUploadJob";

        public BuildingGrbUploadJobToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
