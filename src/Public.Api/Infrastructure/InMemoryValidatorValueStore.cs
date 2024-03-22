namespace Public.Api.Infrastructure
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Extensions.Logging;

    public class InMemoryValidatorValueStore : IValidatorValueStore
    {
        private readonly ILogger<InMemoryValidatorValueStore> _logger;

        private readonly ConcurrentDictionary<string, ValidatorValue> _store
            = new ConcurrentDictionary<string, ValidatorValue>();

        public InMemoryValidatorValueStore(ILogger<InMemoryValidatorValueStore> logger) => _logger = logger;

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            if (key.Values.Any(x => x.Contains("/docs")))
                return null;

            _logger.LogDebug("Checking InMemory for key '{key}'", key.ToString());

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

        public Task<bool> RemoveAsync(StoreKey key)
        {
            // We dont invalidate anything in public api
            return Task.FromResult(true);
        }

        public IAsyncEnumerable<StoreKey> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch)
        {
            // We dont use this in public api
            return Task.FromResult(new List<StoreKey>().AsEnumerable());
        }
    }
}
