namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Common.Infrastructure;
    using Feeds.V2;
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
        private readonly FeedsVisibleToggle _feedsVisibleToggle;

        public ToggledApiControllerSpec(FeedsVisibleToggle feedsVisibleToggle)
        {
            _feedsVisibleToggle = feedsVisibleToggle;
        }

        public bool IsSatisfiedBy(ControllerModel controller)
        {
            var apiVisibility = controller
                .ControllerType
                .GetCustomAttributes<ApiVisibleAttribute>(true)
                .Select(x => x.Visible)
                .ToList();

            if (controller.ApiExplorer.GroupName == FeedV2Controller.FeedsGroupName)
                return _feedsVisibleToggle.FeatureEnabled;

            return apiVisibility.Count != 0 && apiVisibility.First();
        }
    }
}
