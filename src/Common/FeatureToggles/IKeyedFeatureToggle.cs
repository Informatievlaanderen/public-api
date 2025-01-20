namespace Common.FeatureToggles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FeatureToggle;
    using Microsoft.Extensions.DependencyInjection;

    public interface IKeyedFeatureToggle : IFeatureToggle
    {
        string Key { get; }
    }

    public static class KeyedFeatureToggleExtensions
    {
        public static IServiceCollection RegisterFeatureToggles(this IServiceCollection serviceCollection)
        {
            var applicationFeatureToggleType = typeof(IKeyedFeatureToggle);

            // Find all types that implement IKeyedFeatureToggle
            var toggleTypes = applicationFeatureToggleType.Assembly
                .GetTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false } && applicationFeatureToggleType.IsAssignableFrom(type))
                .ToList();

            foreach (var toggleType in toggleTypes)
            {
                serviceCollection.AddSingleton(toggleType, sp => ActivatorUtilities.CreateInstance(sp, toggleType));
                serviceCollection.AddSingleton(typeof(IKeyedFeatureToggle), sp => sp.GetRequiredService(toggleType));
            }

            return serviceCollection;
        }

        public static IEnumerable<IKeyedFeatureToggle> GetFeatureToggles(IDynamicFeatureToggleService service)
        {
            var applicationFeatureToggleType = typeof(IKeyedFeatureToggle);

            return applicationFeatureToggleType.Assembly
                .GetTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false } && applicationFeatureToggleType.IsAssignableFrom(type))
                .Select(x => (IKeyedFeatureToggle)Activator.CreateInstance(x, service))
                .ToList();
        }
    }

    public abstract class KeyedFeatureToggleBase : IKeyedFeatureToggle
    {
        public abstract string Key { get; }
        public bool FeatureEnabled { get; }

        protected KeyedFeatureToggleBase(IDynamicFeatureToggleService? dynamicFeatureToggleService)
        {
            FeatureEnabled = dynamicFeatureToggleService?.IsFeatureEnabled(Key) ?? false;
        }
    }
}
