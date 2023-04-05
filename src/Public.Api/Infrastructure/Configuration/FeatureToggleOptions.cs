namespace Public.Api.Infrastructure.Configuration
{
    public class FeatureToggleOptions
    {
        public const string ConfigurationKey = "FeatureToggles";
        public bool IsFeedsVisible { get; set; }

        public bool ProposeStreetName { get; set; }
        public bool ApproveStreetName { get; set; }
        public bool RejectStreetName { get; set; }
        public bool RetireStreetName { get; set; }
        public bool RemoveStreetName { get; set; }
        public bool CorrectStreetNameRetirement { get; set; }
        public bool CorrectStreetNameNames { get; set; }

        public bool CorrectStreetNameHomonymAdditions { get; set; }
        public bool CorrectStreetNameApproval { get; set; }
        public bool CorrectStreetNameRejection { get; set; }

        public bool ProposeAddress { get; set; }
        public bool ApproveAddress { get; set; }
        public bool DeregulateAddress { get; set; }
        public bool RegularizeAddress { get; set; }
        public bool RejectAddress { get; set; }
        public bool RetireAddress { get; set; }
        public bool RemoveAddress { get; set; }
        public bool ChangePostalCodeAddress { get; set; }
        public bool ChangePositionAddress { get; set; }
        public bool CorrectHouseNumberAddress { get; set; }
        public bool CorrectBoxNumberAddress { get; set; }
        public bool CorrectPostalCodeAddress { get; set; }
        public bool CorrectPositionAddress { get; set; }
        public bool CorrectApprovalAddress { get; set; }
        public bool CorrectRejectionAddress { get; set; }
        public bool CorrectRetirementAddress { get; set; }
        public bool CorrectRegularizationAddress { get; set; }
        public bool CorrectDeregulationAddress { get; set; }
        public bool ReaddressStreetNameAddresses { get; set; }

        public bool PlanBuilding { get; set; }
        public bool BuildingUnderConstruction { get; set; }
        public bool CorrectBuildingUnderConstruction { get; set; }
        public bool RealizeBuilding { get; set; }
        public bool CorrectBuildingRealization { get; set; }
        public bool NotRealizeBuilding { get; set; }
        public bool CorrectBuildingNotRealization { get; set; }
        public bool ChangeBuildingGeometryOutline { get; set; }
        public bool DemolishBuilding { get; set; }
        public bool RemoveBuilding { get; set; }
        public bool ChangeGeometryBuilding { get; set; }
        public bool CorrectGeometryBuilding { get; set; }
        public bool BuildingGrbUploadJob { get; set; }

        public bool PlanBuildingUnit { get; set; }
        public bool RealizeBuildingUnit { get; set; }
        public bool CorrectBuildingUnitRealization { get; set; }
        public bool NotRealizeBuildingUnit { get; set; }
        public bool CorrectBuildingUnitNotRealization { get; set; }
        public bool RetireBuildingUnit { get; set; }
        public bool CorrectBuildingUnitRetirement { get; set; }
        public bool AttachAddressBuildingUnit { get; set; }
        public bool DetachAddressBuildingUnit { get; set; }
        public bool RegularizeBuildingUnit { get; set; }
        public bool CorrectBuildingUnitRegularization { get; set; }
        public bool DeregulateBuildingUnit { get; set; }
        public bool CorrectBuildingUnitDeregulation { get; set; }
        public bool ChangeFunctionBuildingUnit { get; set; }
        public bool CorrectFunctionBuildingUnit { get; set; }
        public bool CorrectBuildingUnitPosition { get; set; }
        public bool RemoveBuildingUnit { get; set; }

        public bool AttachAddressParcel { get; set; }
        public bool DetachAddressParcel { get; set; }

        public bool IsAddressOsloApiEnabled { get; set; }
        public bool IsBuildingOsloApiEnabled { get; set; }
        public bool IsBuildingUnitOsloApiEnabled { get; set; }
        public bool IsMunicipalityOsloApiEnabled { get; set; }
        public bool IsParcelOsloApiEnabled { get; set; }
        public bool IsPostalCodeOsloApiEnabled { get; set; }
        public bool IsStreetNameOsloApiEnabled { get; set; }

        public bool Ticketing { get; set; }

        public bool RoadSegmentLinkStreetName { get; set; }
        public bool RoadSegmentUnlinkStreetName { get; set; }
        public bool RoadSegmentGet { get; set; }
    }
}
