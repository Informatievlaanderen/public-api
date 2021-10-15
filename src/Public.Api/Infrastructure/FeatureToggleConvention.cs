namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.Extensions.Configuration;

    public class FeatureToggleConvention : IActionModelConvention
    {
        private readonly Dictionary<string, bool> _features;
        public FeatureToggleConvention([FromServices] IConfiguration configuration)
        {
            _features = configuration.GetSection(FeatureToggleOptions.ConfigurationKey).GetChildren().AsEnumerable()
                .ToDictionary(x => x.Key, y => Convert.ToBoolean(y.Value));
        }

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible.HasValue && !action.ApiExplorer.IsVisible.Value)
             return;
            
            action.ApiExplorer.IsVisible =  !_features.ContainsKey(action.ActionName) ||
                                           (_features.ContainsKey(action.ActionName) && _features[action.ActionName]);
        }
    }
}
