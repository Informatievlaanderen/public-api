namespace Common.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage AddHeaderAuthorization(this HttpRequestMessage request, IActionContextAccessor actionContextAccessor)
        {
            if (actionContextAccessor.ActionContext.HttpContext.Request.Headers.Authorization.Any())
            {
                var authHeaderValueParts = actionContextAccessor.ActionContext.HttpContext.Request.Headers.Authorization.First()!.Split(" ");
                request.Headers.Authorization = new AuthenticationHeaderValue(authHeaderValueParts[0], string.Join(" ", authHeaderValueParts.Skip(1)));
            }

            return request;
        }
    }
}
