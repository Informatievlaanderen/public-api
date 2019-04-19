namespace Public.Api.Infrastructure.Configuration
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public interface IRegistryOptions
    {
        SyndicationOptions Syndication { get; set; }
    }

    /// <summary>
    /// Simplifies Configuring an options class and its parent.
    /// </summary>
    public static class OptionsConfigurationExtensions
    {
        public static IServiceCollection ConfigureRegistryOptions<T>(
            this IServiceCollection services,
            IConfiguration configuration) where T : class, IRegistryOptions
            => ((IServiceCollection)typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod(
                        nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                        new[] { typeof(IServiceCollection), typeof(IConfiguration) })
                    .MakeGenericMethod(typeof(T).BaseType)
                    .Invoke(null, new object[] { services, configuration }))
                .Configure<T>(configuration);
    }
}
