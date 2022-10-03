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
        public bool CorrectStreetNameNames { get; set; }

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
        public bool CorrectRejectionAddress { get; set; }
        
        public bool PlanBuilding { get; set; }
        public bool BuildingUnderConstruction { get; set; }
        public bool RealizeBuilding { get; set; }
        public bool NotRealizeBuilding { get; set; }
        public bool DemolishBuilding { get; set; }
        public bool ChangeGeometryBuilding { get; set; }
        public bool CorrectGeometryBuilding { get; set; }

        public bool PlanBuildingUnit { get; set; }
        public bool RealizeBuildingUnit { get; set; }
        public bool NotRealizeBuildingUnit { get; set; }
        public bool RetireBuildingUnit { get; set; }
        public bool AttachAddressBuildingUnit { get; set; }
        public bool DetachAddressBuildingUnit { get; set; }
        public bool RegularizeBuildingUnit { get; set; }
        public bool DeregulateBuildingUnit { get; set; }
        public bool ChangeFunctionBuildingUnit { get; set; }
        public bool CorrectFunctionBuildingUnit { get; set; }

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
    }
}
