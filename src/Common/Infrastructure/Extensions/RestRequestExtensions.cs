namespace Common.Infrastructure.Extensions
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        public static RestRequest AddHeaderAuthorization(this RestRequest request, IActionContextAccessor actionContextAccessor)
        {
            if (actionContextAccessor.ActionContext.HttpContext.Request.Headers.Authorization.Any())
            {
                request.AddHeader(HeaderNames.Authorization, actionContextAccessor.ActionContext.HttpContext.Request.Headers.Authorization);
            }

            return request;
        }
    }
}
