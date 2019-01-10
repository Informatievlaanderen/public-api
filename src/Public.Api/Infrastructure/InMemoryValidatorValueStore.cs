namespace Public.Api.Infrastructure
{
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Linq;

    public class InMemoryValidatorValueStore : IValidatorValueStore
    {
        private readonly ConcurrentDictionary<string, ValidatorValue> _store
            = new ConcurrentDictionary<string, ValidatorValue>();

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            if (key.Values.Any(x => x.Contains("/docs")))
                return null;

            return Get(key.ToString());
        }

        public async Task SetAsync(StoreKey key, ValidatorValue eTag)
        {
            if (key.Values.Any(x => x.Contains("/docs/")))
                return;

            Set(key.ToString(), eTag);
        }
        private ValidatorValue Get(string key)
            => _store.ContainsKey(key) && _store[key] is ValidatorValue eTag
                ? eTag
                : null;

        private void Set(string key, ValidatorValue eTag) => _store[key] = eTag;
    }
}
