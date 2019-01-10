namespace Public.Api.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;

    public static class HttpRequestExtensions
    {
        public static string GetValueFromRouteData(this ActionContext context, string key)
        {
            if (!context.RouteData.Values.TryGetValue(key, out var value))
                return null;

            var routeValue = value?.ToString();

            return string.IsNullOrEmpty(routeValue) ? null : routeValue.ToLowerInvariant();
        }

        public static string GetValueFromQueryString(this ActionContext context, string key)
            => context.HttpContext.Request.Query.TryGetValue(key, out var queryValue) ? queryValue.ToString().ToLowerInvariant() : null;

        public static string GetValueFromHeader(this ActionContext context, string key)
            => context.HttpContext.Request.Headers.TryGetValue(key, out var headerValue) ? headerValue.ToString().ToLowerInvariant() : null;
    }
}
