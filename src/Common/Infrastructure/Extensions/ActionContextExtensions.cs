namespace Common.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc;

    public static class ActionContextExtensions
    {
        public static string DetermineFormatParameter(this ActionContext actionContext, string format)
            => !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContext.DetermineFormatParameter();

        public static string DetermineFormatParameter(this ActionContext actionContext)
            => actionContext.GetValueFromHeader("format")
               ?? actionContext.GetValueFromRouteData("format")
               ?? actionContext.GetValueFromQueryString("format");
    }
}
