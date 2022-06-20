namespace Public.Api.Infrastructure.Configuration
{
    public class FeatureToggleOptions
    {
        public const string ConfigurationKey = "FeatureToggles";
        public bool IsFeedsVisible { get; set; }
        public bool ProposeStreetName { get; set; }
        public bool ApproveStreetName { get; set; }
        public bool ProposeAddress { get; set; }
        public bool PlanBuilding { get; set; }
        public bool BuildingUnderConstruction { get; set; }
        public bool RealizeBuilding { get; set; }
        public bool IsAddressOsloApiEnabled { get; set; }
        public bool IsBuildingOsloApiEnabled { get; set; }
        public bool IsBuildingUnitOsloApiEnabled { get; set; }
        public bool IsMunicipalityOsloApiEnabled { get; set; }
        public bool IsParcelOsloApiEnabled { get; set; }
        public bool IsPostalCodeOsloApiEnabled { get; set; }
        public bool IsStreetNameOsloApiEnabled { get; set; }
    }
}
