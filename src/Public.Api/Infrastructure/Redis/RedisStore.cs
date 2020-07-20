namespace Public.Api.Infrastructure.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;

    public class RedisStore : IValidatorValueStore
    {
        private const string ETagKey = "eTag";
        private const string ETagTypeKey = "eTagType";
        private const string LastModifiedKey = "lastModified";
        private const string SetByRegistryKey = "setByRegistry";
        private const string HmsetnxScript = @"
if redis.call('exists', KEYS[1]) == 0 then
    redis.call('hmset', KEYS[1], unpack(ARGV))
end
";

        private readonly ILogger<RedisStore> _logger;
        private readonly IConnectionMultiplexer _redis;

        public RedisStore(
            ILogger<RedisStore> logger,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            if (!ShouldCacheValue(key.Values.FirstOrDefault()))
                return null;

            _logger.LogDebug("Checking Redis for key '{key}'", key.ToString());

            var db = _redis.GetDatabase();

            var redisValues =
                await db.HashGetAsync(key.ToString(), new RedisValue[] { ETagKey, ETagTypeKey, LastModifiedKey, SetByRegistryKey }, CommandFlags.PreferReplica);

            if (redisValues.Length != 4 ||
                redisValues[0].IsNullOrEmpty ||
                redisValues[1].IsNullOrEmpty ||
                redisValues[2].IsNullOrEmpty ||
                redisValues[3].IsNullOrEmpty)
                return null;

            var setByRegistry = bool.Parse(redisValues[3].ToString());
            if (!setByRegistry)
                return null;

            var eTag = redisValues[0].ToString();
            var eTagType = Enum.Parse<ETagType>(redisValues[1].ToString(), true);

            var lastModified = DateTimeOffset.ParseExact(
                redisValues[2].ToString(),
                "O",
                CultureInfo.InvariantCulture);

            return new ValidatorValue(new ETag(eTagType, eTag), lastModified);
        }

        public async Task SetAsync(StoreKey key, ValidatorValue eTag)
        {
            if (!ShouldCacheValue(key.Values.FirstOrDefault()))
                return;

            var db = _redis.GetDatabase();

            var hashValues = new []
            {
                ETagKey, eTag.ETag.Value,
                ETagTypeKey, eTag.ETag.ETagType.ToString(),
                LastModifiedKey, eTag.LastModified.ToString("O", CultureInfo.InvariantCulture),
                SetByRegistryKey, false.ToString(CultureInfo.InvariantCulture)
            };

            await db.ScriptEvaluateAsync(HmsetnxScript,
                new RedisKey[] { key.ToString() },
                Array.ConvertAll(hashValues, value => (RedisValue)value),
                CommandFlags.FireAndForget);
        }

        private static bool ShouldCacheValue(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            path = path.ToLowerInvariant();

            return RedisStoreKeyGenerator.CachePrefixes.Any(prefix => path.Contains(prefix));
        }

        public Task<bool> RemoveAsync(StoreKey key)
        {
            // We dont invalidate anything in public api
            return Task.FromResult(true);
        }

        public Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch)
        {
            // We dont use this in public api
            return Task.FromResult(new List<StoreKey>().AsEnumerable());
        }
    }
}
