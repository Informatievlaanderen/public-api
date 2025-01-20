namespace Common.FeatureToggles
{
    public sealed class FeedsVisibleToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "FeedsVisible";

        public FeedsVisibleToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class TicketingToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "Ticketing";

        public TicketingToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ListSuspiciousCasesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "SuspiciousCasesList";

        public ListSuspiciousCasesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DetailSuspiciousCasesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "SuspiciousCasesDetail";

        public DetailSuspiciousCasesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
