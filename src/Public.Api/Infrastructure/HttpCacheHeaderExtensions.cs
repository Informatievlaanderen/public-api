namespace Public.Api.Infrastructure
{
    using System;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Builder;

    public static class HttpCacheHeaderExtensions
    {
        public static IApplicationBuilder UseConditionalHttpCacheHeaders(
            this IApplicationBuilder builder)
        {
            if (builder != null)
                return builder.UseMiddleware<HttpCacheHeaderWithExclusionsMiddleware>();
            throw new ArgumentNullException(nameof(builder));
        }
    }
}
