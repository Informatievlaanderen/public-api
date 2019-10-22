namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public static class ApiOrder
    {
        public const int Address = 10;
        public const int AddressRepresentation = 10;
        public const int Building = 10;
        public const int BuildingUnit = 10;
        public const int CrabHouseNumber = 10;
        public const int CrabSubaddress = 10;
        public const int Extract = 10;
        public const int Feeds = 10;
        public const int Municipality = 10;
        public const int Parcel = 10;
        public const int PostalCode = 10;
        public const int PublicService = 10;
        public const int StreetName = 10;
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
