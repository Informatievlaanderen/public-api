namespace Public.Api.Infrastructure
{
    using System;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtensions
    {
        public static bool IsHtmlRequest(this HttpRequest request)
            => request
                .Headers[HeaderNames.Accept]
                .ToString()
                .Contains("text/html", StringComparison.InvariantCultureIgnoreCase);
    }
}
