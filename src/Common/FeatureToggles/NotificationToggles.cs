namespace Common.FeatureToggles
{
    public sealed class CreateNotificationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "CreateNotification";

        public CreateNotificationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeleteNotificationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "DeleteNotification";

        public DeleteNotificationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class PublishNotificationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "PublishNotification";

        public PublishNotificationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class UnpublishNotificationToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "UnpublishNotification";

        public UnpublishNotificationToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetNotificationsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "GetNotifications";

        public GetNotificationsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetNotificationsByPlatformToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "GetNotificationsByPlatform";

        public GetNotificationsByPlatformToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
