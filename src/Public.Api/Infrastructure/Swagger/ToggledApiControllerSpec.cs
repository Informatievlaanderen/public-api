namespace Public.Api.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Feeds;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.Extensions.Options;

    [AttributeUsage(AttributeTargets.Class)]
    public class ApiVisibleAttribute : Attribute
    {
        public bool Visible { get; }

        public ApiVisibleAttribute() : this(true) { }
        public ApiVisibleAttribute(bool visible) => Visible = visible;
    }

    public class ToggledApiControllerSpec : IApiControllerSpecification
    {
        private readonly FeatureToggleOptions _featureToggleOptions;

        public ToggledApiControllerSpec(IOptions<FeatureToggleOptions> featureToggleOptions)
        {
            _featureToggleOptions = featureToggleOptions.Value;
        }

        public bool IsSatisfiedBy(ControllerModel controller)
        {
            var apiVisibility = controller
                .ControllerType
                .GetCustomAttributes<ApiVisibleAttribute>(true)
                .Select(x => x.Visible)
                .ToList();

            if (controller.ApiExplorer.GroupName == FeedController.FeedsGroupName)
                return _featureToggleOptions.IsFeedsVisible;

            return apiVisibility.Count != 0 && apiVisibility.First();
        }
    }
}
