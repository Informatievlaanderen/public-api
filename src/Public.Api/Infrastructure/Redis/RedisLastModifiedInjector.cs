namespace Public.Api.Infrastructure.Redis
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Domain;
    using Marvin.Cache.Headers.Interfaces;

    public class RedisLastModifiedInjector : DefaultLastModifiedInjector, ILastModifiedInjector
    {
        public new Task<DateTimeOffset> CalculateLastModified(ResourceContext context)
        {
            if (context.ValidatorValue != null)
                return Task.FromResult(context.ValidatorValue.LastModified);

            if (context.HttpRequest.HttpContext.Response.Headers.ContainsKey("x-last-modified"))
                return Task.FromResult(DateTimeOffset.ParseExact(
                    context.HttpRequest.HttpContext.Response.Headers["x-last-modified"],
                    "O",
                    CultureInfo.InvariantCulture));

            return base.CalculateLastModified(context);
        }
    }
}
