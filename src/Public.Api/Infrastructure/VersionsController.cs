namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get(
            [FromServices] HealthUrls healthUrls,
            [FromServices] ILifetimeScope scope,
            CancellationToken cancellationToken = default)
        {
            var versions = new Dictionary<string, string>();

            foreach (var (registry, _) in healthUrls)
            {
                var healthClient = scope.ResolveNamed<IRestClient>($"Health-{registry}");
                var healthResponse = await healthClient.ExecuteTaskAsync(new RestRequest(), cancellationToken);

                var downstreamVersion = healthResponse
                    ?.Headers
                    ?.FirstOrDefault(x => x.Name.Equals(AddVersionHeaderMiddleware.HeaderName, StringComparison.InvariantCultureIgnoreCase))
                    ?.Value
                    ?.ToString();

                versions.Add(registry, string.IsNullOrWhiteSpace(downstreamVersion) ? "Unknown" : downstreamVersion);
            }
            
            return Ok(versions);
        }
    }
}
