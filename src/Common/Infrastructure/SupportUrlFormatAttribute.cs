namespace Public.Api.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Common.Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SupportUrlFormatAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.SetContentFormatAcceptType();

            await next();
        }
    }
}
