namespace Public.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.FeatureToggles;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    public sealed class FeatureToggleConvention : IActionModelConvention
    {
        private readonly Dictionary<string, bool> _features;

        public FeatureToggleConvention(IEnumerable<IKeyedFeatureToggle> featureToggles)
        {
            _features = featureToggles.ToDictionary(x => x.GetType().FullName, x => x.FeatureEnabled);
        }

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible.HasValue && !action.ApiExplorer.IsVisible.Value)
            {
                return;
            }

            var toggleParameter = action
                .ActionMethod
                .GetParameters()
                .FirstOrDefault(param => typeof(IKeyedFeatureToggle).IsAssignableFrom(param.ParameterType));

            if (toggleParameter != null)
            {
                action.ApiExplorer.IsVisible = _features.ContainsKey(toggleParameter.ParameterType.FullName)
                                               && _features[toggleParameter.ParameterType.FullName];
                return;
            }

            action.ApiExplorer.IsVisible = true;
        }
    }
}
