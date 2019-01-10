namespace Public.Api.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
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

        private readonly ConnectionMultiplexer _redis;

        public RedisStore(ConnectionMultiplexer redis) => _redis = redis;

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            if (key.Values.Any(x => x.Contains("/docs")))
                return null;

            var db = _redis.GetDatabase();

            var redisValues =
                await db.HashGetAsync(key.ToString(), new RedisValue[] { ETagKey, ETagTypeKey, LastModifiedKey, SetByRegistryKey }, CommandFlags.PreferSlave);

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
            if (key.Values.Any(x => x.Contains("/docs/")))
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
    }
}
