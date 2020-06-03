namespace Public.Api.Infrastructure.Version
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Common.Infrastructure.Modules;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    [ApiVersionNeutral]
    [Route("versions")]
    public class VersionsController : ApiController
    {
        private readonly MarketingVersion _version;

        public VersionsController(MarketingVersion version) => _version = version;

        private const string OldVersionHeaderName = "x-basisregister-version";

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get(
            [FromServices] HealthUrls healthUrls,
            [FromServices] ILifetimeScope scope,
            CancellationToken cancellationToken = default)
        {
            var components = new ConcurrentDictionary<string, string>();

            components.TryAdd("publicApi", FormatVersion(Assembly.GetEntryAssembly().GetName().Version.ToString(4)));

            await Task.WhenAll(healthUrls.Select(url => GetDownstreamVersionAsync(url.Key, scope, components, cancellationToken)));

            return Ok(new ApiVersionResponse(_version, components));
        }

        private static string FormatVersion(string fourPartVersion) => string.Join(".", fourPartVersion.Split(".").Skip(1));

        private static async Task GetDownstreamVersionAsync(
            string registry,
            IComponentContext scope,
            ConcurrentDictionary<string, string> versions,
            CancellationToken cancellationToken)
        {
            var healthClient = scope.ResolveNamed<IRestClient>($"Health-{registry}");
            var healthResponse = await healthClient.ExecuteAsync(new RestRequest(), cancellationToken);

            var downstreamVersion = healthResponse
                ?.Headers
                ?.FirstOrDefault(header =>
                    header.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase) ||
                    header.Name.Equals(OldVersionHeaderName, StringComparison.InvariantCultureIgnoreCase))
                ?.Value
                ?.ToString();

            versions.TryAdd(registry, string.IsNullOrWhiteSpace(downstreamVersion) ? "Unknown" : FormatVersion(downstreamVersion));
        }
    }
}
