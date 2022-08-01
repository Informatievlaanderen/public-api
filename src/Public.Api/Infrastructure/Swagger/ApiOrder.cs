namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public static class ApiOrder
    {
        public const int Municipality = 100;
        public const int PostalCode = Municipality + 5;
        public const int StreetName = PostalCode + 5;
        public const int Address = StreetName + 5;
        public const int Building = Address + 5;
        public const int BuildingUnit = Building + 5;
        public const int Parcel = BuildingUnit + 5;
        public const int PublicService = Parcel + 5;
        public const int TicketingService = PublicService + 5;

        public const int RoadChangeFeed = 150;
        public const int RoadDownload = RoadChangeFeed + 1;
        public const int RoadExtract = RoadDownload + 1;
        public const int RoadInformation = RoadExtract + 1;
        public const int RoadUpload = RoadInformation + 1;

        public const int Extract = 160;
        public const int Feeds = Extract + 5;

        public const int CrabHouseNumber = 170;
        public const int CrabSubaddress = CrabHouseNumber + 5;
        public const int CrabBuildings = CrabSubaddress + 5;

        public const int Status = 300;

        public const int AddressRepresentation = 500;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ApiOrderAttribute : Attribute
    {
        public int Order { get; set; }
    }

    public static class SortByApiOrder
    {
        public static string Sort(ApiDescription desc)
        {
            if (!(desc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                return string.Empty;

            var apiGroupNames = controllerActionDescriptor
                .ControllerTypeInfo
                .GetCustomAttributes<ApiOrderAttribute>(true)
                .Select(x => x.Order)
                .ToList();

            return apiGroupNames.Count == 0
                ? int.MaxValue.ToString()
                : apiGroupNames.First().ToString();
        }
    }
}
