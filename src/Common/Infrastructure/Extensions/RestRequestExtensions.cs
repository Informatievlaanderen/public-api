namespace Common.Infrastructure.Extensions
{
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using RestSharp;

    public static class RestRequestExtensions
    {
        public static RestRequest AddHeaderIfMatch(this RestRequest request, string? ifMatch)
        {
            if (ifMatch is not null)
            {
                request.AddHeader(HeaderNames.IfMatch, ifMatch);
            }

            return request;
        }

        public static RestRequest AddHeaderAuthorization(this RestRequest request, IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Request.Headers.Authorization.Any())
            {
                request.AddHeader(HeaderNames.Authorization, httpContextAccessor.HttpContext.Request.Headers.Authorization);
            }

            return request;
        }
    }
}
