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
        public const int PostalCode = 105;
        public const int StreetName = 110;
        public const int Address = 115;
        public const int Building = 120;
        public const int BuildingUnit = 125;
        public const int Parcel = 130;
        public const int PublicService = 135;

        public const int RoadChangeFeed = 136;
        public const int RoadDownload = 137;
        public const int RoadExtract = 138;
        public const int RoadInformation = 139;

        public const int Extract = 140;
        public const int Feeds = 145;

        public const int CrabHouseNumber = 150;
        public const int CrabSubaddress = 155;
        public const int CrabBuildings = 159;

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
