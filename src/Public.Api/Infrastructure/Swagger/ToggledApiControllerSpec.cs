namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    [AttributeUsage(AttributeTargets.Class)]
    public class ApiVisibleAttribute : Attribute
    {
        public bool Visible { get; }

        public ApiVisibleAttribute() : this(true) { }
        public ApiVisibleAttribute(bool visible) => Visible = visible;
    }

    public class ToggledApiControllerSpec : IApiControllerSpecification
    {
        public bool IsSatisfiedBy(ControllerModel controller)
        {
            var apiVisibility = controller
                .ControllerType
                .GetCustomAttributes<ApiVisibleAttribute>(true)
                .Select(x => x.Visible)
                .ToList();

            return apiVisibility.Count != 0 && apiVisibility.First();
        }
    }
}
