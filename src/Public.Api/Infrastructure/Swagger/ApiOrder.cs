namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public static class ApiOrder
    {
        public static class Municipality
        {
            public const int Base = 1000;
            public const int V2 = Base + 20;
        }

        public static class PostalCode
        {
            public const int Base = Municipality.Base + 100;
            public const int V2 = Base + 20;
        }

        public static class StreetName
        {
            public const int Base = PostalCode.Base + 100;
            public const int V2 = Base + 20;
            public const int Edit = Base + 30;
        }

        public static class Address
        {
            public const int Base = StreetName.Base + 100;
            public const int V2 = Base + 20;
            public const int Edit = Base + 30;
        }

        public static class Building
        {
            public const int Base = Address.Base + 100;
            public const int V2 = Base + 20;
            public const int Edit = Base + 30;
        }

        public static class BuildingUnit
        {
            public const int Base = Building.Base + 100;
            public const int V2 = Base + 20;
            public const int Edit = Base + 30;
        }

        public static class Parcel
        {
            public const int Base = BuildingUnit.Base + 100;
            public const int V2 = Base + 20;
            public const int Edit = Base + 30;
        }

        public static class Road
        {
            public const int ChangeFeed = Parcel.Base + 100;
            public const int Download = ChangeFeed + 10;
            public const int RoadExtract = ChangeFeed + 20;
            public const int Information = ChangeFeed + 30;
            public const int RoadUpload = ChangeFeed + 40;
            public const int Organization = ChangeFeed + 50;
            public static class RoadSegment
            {
                public const int Root = ChangeFeed + 60;

                public const int Get = Root + 1;
                public const int ChangeAttributes = Get + 1;
                public const int ChangeDynamicAttributes = ChangeAttributes + 1;
                public const int CreateOutline = ChangeDynamicAttributes + 1;
                public const int ChangeOutlineGeometry = CreateOutline + 1;
                public const int DeleteOutline = ChangeOutlineGeometry + 1;
                public const int DeleteRoadSegments = DeleteOutline + 1;
                public const int LinkStreetName = DeleteRoadSegments + 1;
                public const int UnlinkStreetName = LinkStreetName + 1;
            }
            public const int RoadGrb = ChangeFeed + 70;
            public const int RoadInwinning = ChangeFeed + 80;
        }

        public const int Extract = Road.ChangeFeed + 100;
        public const int Feeds = Extract + 100;

        public static class SuspiciousCases
        {
            public const int Base = Feeds + 100;
            public const int V2 = Base + 10;
        }

        public const int Status = SuspiciousCases.V2 + 100;

        public const int Notifications = SuspiciousCases.V2 + 400;

        public const int TicketingService = SuspiciousCases.V2 + 500;

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiOrderAttribute : Attribute
    {
        public int Order { get; set; }

        public ApiOrderAttribute(int order)
        {
            Order = order;
        }
    }

    public static class SortByApiOrder
    {
        public static string Sort(ApiDescription desc)
        {
            if (desc.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
            {
                return string.Empty;
            }

            // Order by method
            var methodOrder = controllerActionDescriptor
                .MethodInfo
                .GetCustomAttribute<ApiOrderAttribute>()
                ?.Order;

            if (methodOrder.HasValue)
            {
                return methodOrder.Value.ToString();
            }

            // Order by controller
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
