namespace Public.Api.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Domain;
    using Marvin.Cache.Headers.Interfaces;

    public class RedisLastModifiedInjector : DefaultLastModifiedInjector, ILastModifiedInjector
    {
        public new Task<DateTimeOffset> CalculateLastModified(ResourceContext context)
            => context.ValidatorValue == null
                ? base.CalculateLastModified(context)
                : Task.FromResult(context.ValidatorValue.LastModified);
    }
}
