namespace Public.Api.Infrastructure.Configuration
{
    using System;
    using Microsoft.AspNetCore.Mvc.Infrastructure;

    public class SyndicationOptions
    {
        public string NextUri { get; set; }
        public string NextUriV2 => NextUri.Replace("/v1/", "/v2/");

        public string GetNextUri(IActionContextAccessor actionContext)
        {
            return actionContext.ActionContext.HttpContext.Request.Path.Value.Contains("v2", StringComparison.InvariantCultureIgnoreCase)
                ? NextUriV2
                : NextUri;
        }
    }
}
