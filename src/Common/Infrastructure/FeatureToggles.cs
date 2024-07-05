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

    public class RenameStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RenameStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RemoveStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RemoveStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectStreetNameRetirementToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectStreetNameRetirementToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectStreetNameNamesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectStreetNameNamesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectStreetNameHomonymAdditionsToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectStreetNameHomonymAdditionsToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectStreetNameApprovalToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectStreetNameApprovalToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectStreetNameRejectionToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectStreetNameRejectionToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
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

    public class RemoveAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RemoveAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
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

    public class CorrectPositionAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectPositionAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectApprovalAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectApprovalAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectRejectionAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectRejectionAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectRetirementAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectRetirementAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectRegularizationAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectRegularizationAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectDeregulationAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectDeregulationAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectRemovalAddressToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectRemovalAddressToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ReaddressStreetNameAddressesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ReaddressStreetNameAddressesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class PlanBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public PlanBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class MergeBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public MergeBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class BuildingUnderConstructionToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public BuildingUnderConstructionToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnderConstructionToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnderConstructionToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RealizeBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RealizeBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingRealizationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingRealizationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class NotRealizeBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public NotRealizeBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingNotRealizationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingNotRealizationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeBuildingGeometryOutlineToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeBuildingGeometryOutlineToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RemoveBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RemoveBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
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

    public class CorrectGeometryBuildingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectGeometryBuildingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class BuildingGrbUploadJobToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public BuildingGrbUploadJobToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
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

    public class CorrectBuildingUnitRealizationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitRealizationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class NotRealizeBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public NotRealizeBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitNotRealizationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitNotRealizationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RetireBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RetireBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitRetirementToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitRetirementToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitRemovalToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitRemovalToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitPositionToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitPositionToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RemoveBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RemoveBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class AttachAddressBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public AttachAddressBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DetachAddressBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DetachAddressBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RegularizeBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RegularizeBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitRegularizationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitRegularizationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DeregulateBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DeregulateBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectBuildingUnitDeregulationToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectBuildingUnitDeregulationToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeFunctionBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeFunctionBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CorrectFunctionBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CorrectFunctionBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class MoveBuildingUnitToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public MoveBuildingUnitToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class AttachAddressParcelToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public AttachAddressParcelToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DetachAddressParcelToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DetachAddressParcelToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class TicketingToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public TicketingToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeRoadSegmentAttributesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeRoadSegmentAttributesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeRoadSegmentDynamicAttributesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeRoadSegmentDynamicAttributesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ChangeRoadSegmentOutlineGeometryToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ChangeRoadSegmentOutlineGeometryToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class CreateRoadSegmentOutlineToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public CreateRoadSegmentOutlineToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DeleteRoadSegmentOutlineToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DeleteRoadSegmentOutlineToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class LinkRoadSegmentStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public LinkRoadSegmentStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class UnlinkRoadSegmentStreetNameToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public UnlinkRoadSegmentStreetNameToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class GetRoadSegmentToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public GetRoadSegmentToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class GetRoadOrganizationsToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public GetRoadOrganizationsToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class ListSuspiciousCasesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ListSuspiciousCasesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class DetailSuspiciousCasesToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public DetailSuspiciousCasesToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }

    public class RoadJobsToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public RoadJobsToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
