namespace Common.Infrastructure.Controllers.Attributes
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CleanQueryParametersAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context
                .HttpContext
                .Request;

            request.QueryString = CleanQueryString(request.QueryString);

            await next();
        }

        private static QueryString CleanQueryString(QueryString queryString)
        {
            var xmlCleaned = new Regex(@"&(?:amp;)+", RegexOptions.IgnoreCase)
                .Replace(queryString.ToString(), "&");

            return new QueryString(xmlCleaned);
        }
    }
}
