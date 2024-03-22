namespace Public.Api.Infrastructure
{
    using System.Reflection;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersionNeutral]
    [Route("")]
    public class EmptyController : ApiController
    {
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get()
            => Request.IsHtmlRequest()
                ? (IActionResult) new RedirectResult("/docs")
                : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Api {Assembly.GetEntryAssembly().GetVersionText()}.");
    }
}
