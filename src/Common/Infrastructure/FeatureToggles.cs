namespace Common.Infrastructure
{
    using FeatureToggle;

    public class FeedsVisibleToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public FeedsVisibleToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ProposeStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ProposeStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ApproveStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApproveStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ProposeAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ProposeAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class PlanBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public PlanBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class BuildingUnderConstructionToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public BuildingUnderConstructionToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RealizeBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RealizeBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsAddressOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsAddressOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsBuildingOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsBuildingOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsBuildingUnitOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsBuildingUnitOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsMunicipalityOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsMunicipalityOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsParcelOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsParcelOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsPostalCodeOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsPostalCodeOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class IsStreetNameOsloApiEnabledToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public IsStreetNameOsloApiEnabledToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
