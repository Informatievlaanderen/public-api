namespace Common.Infrastructure.Extensions
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage AddHeaderAuthorization(this HttpRequestMessage request, IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Request.Headers.Authorization.Any())
            {
                var authHeaderValueParts = httpContextAccessor.HttpContext.Request.Headers.Authorization.First()!.Split(" ");
                request.Headers.Authorization = new AuthenticationHeaderValue(authHeaderValueParts[0], string.Join(" ", authHeaderValueParts.Skip(1)));
            }

            return request;
        }
    }
}
