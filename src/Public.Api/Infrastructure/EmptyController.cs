namespace Public.Api.Infrastructure
{
    using System.Reflection;
    using System.Threading;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    [ApiVersionNeutral]
    [Route("")]
    public class EmptyController : ControllerBase
    {
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get(
            [FromServices] IHostingEnvironment hostingEnvironment,
            CancellationToken cancellationToken)
        {
            return Request.Headers[HeaderNames.Accept].ToString().Contains("text/html")
                ? (IActionResult) new RedirectResult("/docs")
                : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Api v{Assembly.GetEntryAssembly().GetName().Version}.");
        }
    }
}
