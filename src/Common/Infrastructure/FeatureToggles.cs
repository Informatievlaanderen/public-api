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

    public class RejectStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RejectStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RetireStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RetireStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ProposeAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ProposeAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ApproveAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApproveAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DeregulateAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DeregulateAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RegularizeAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RegularizeAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RejectAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RejectAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RetireAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RetireAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangePostalCodeAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangePostalCodeAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangePositionAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangePositionAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectHouseNumberAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectHouseNumberAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBoxNumberAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBoxNumberAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectPostalCodeAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectPostalCodeAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectPositionAddress : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectPositionAddress(bool featureEnabled) => FeatureEnabled = featureEnabled;
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

    public class NotRealizeBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public NotRealizeBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DemolishBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DemolishBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeGeometryBuilding : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeGeometryBuilding(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectGeometryBuilding : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectGeometryBuilding(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class PlanBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public PlanBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RealizeBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RealizeBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class NotRealizeBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public NotRealizeBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RetireBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RetireBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class AttachAddressBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public AttachAddressBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DetachAddressBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DetachAddressBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RegularizeBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RegularizeBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DeregulateBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DeregulateBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeFunctionBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeFunctionBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectFunctionBuildingUnit : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectFunctionBuildingUnit(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class AttachAddressParcel : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public AttachAddressParcel(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DetachAddressParcel : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DetachAddressParcel(bool featureEnabled) => FeatureEnabled = featureEnabled;
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
