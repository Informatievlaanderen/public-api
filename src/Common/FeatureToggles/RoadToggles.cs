namespace Common.FeatureToggles
{
    public sealed class ChangeRoadSegmentAttributesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeAttributes";

        public ChangeRoadSegmentAttributesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeRoadSegmentDynamicAttributesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeDynamicAttributes";

        public ChangeRoadSegmentDynamicAttributesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeRoadSegmentOutlineGeometryToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeOutlineGeometry";

        public ChangeRoadSegmentOutlineGeometryToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CreateRoadSegmentOutlineToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentCreateOutline";

        public CreateRoadSegmentOutlineToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeleteRoadSegmentOutlineToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentDeleteOutline";

        public DeleteRoadSegmentOutlineToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class LinkRoadSegmentStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentLinkStreetName";

        public LinkRoadSegmentStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class UnlinkRoadSegmentStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentUnlinkStreetName";

        public UnlinkRoadSegmentStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetRoadSegmentToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentGet";

        public GetRoadSegmentToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetRoadOrganizationsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadGetOrganizations";

        public GetRoadOrganizationsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadJobsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadJobs";

        public RoadJobsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
