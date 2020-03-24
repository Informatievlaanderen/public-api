namespace Public.Api.Infrastructure
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
        private const string OldVersionHeaderName = "x-basisregister-version";

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get(
            [FromServices] HealthUrls healthUrls,
            [FromServices] ILifetimeScope scope,
            CancellationToken cancellationToken = default)
        {
            var versions = new ConcurrentDictionary<string, string>();

            versions.TryAdd("publicApi", FormatVersion(Assembly.GetEntryAssembly().GetName().Version.ToString(4)));

            await Task.WhenAll(healthUrls.Select(x => GetDownstreamVersionAsync(x.Key, scope, versions, cancellationToken)));

            return Ok(versions.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value));
        }

        private static string FormatVersion(string fourPartVersion) => string.Join(".", fourPartVersion.Split(".").Skip(1));

        private static async Task GetDownstreamVersionAsync(
            string registry,
            IComponentContext scope,
            ConcurrentDictionary<string, string> versions,
            CancellationToken cancellationToken)
        {
            var healthClient = scope.ResolveNamed<IRestClient>($"Health-{registry}");
            var healthResponse = await healthClient.ExecuteTaskAsync(new RestRequest(), cancellationToken);

            var downstreamVersion = healthResponse
                ?.Headers
                ?.FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase) || x.Name.Equals(OldVersionHeaderName, StringComparison.InvariantCultureIgnoreCase))
                ?.Value
                ?.ToString();

            versions.TryAdd(registry, string.IsNullOrWhiteSpace(downstreamVersion) ? "Unknown" : FormatVersion(downstreamVersion));
        }
    }
}
