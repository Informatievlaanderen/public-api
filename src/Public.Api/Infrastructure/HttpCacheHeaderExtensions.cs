namespace Public.Api.Infrastructure
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;

    public static class HttpCacheHeaderExtensions
    {
        private static readonly string[] DontUseHttpCacheHeadersForPathsWithTheseParts =
        {
            "/wegen/"
        };

        public static IApplicationBuilder UseConditionalHttpCacheHeaders(
            this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.MapWhen(ctx =>
            {
                var requestPath = ctx.Request.Path;
                return !DontUseHttpCacheHeadersForPathsWithTheseParts.Any(part => requestPath.Value.Contains(part, StringComparison.InvariantCultureIgnoreCase));

            }, builder => builder.UseHttpCacheHeaders());

            return applicationBuilder;
        }
    }
}
