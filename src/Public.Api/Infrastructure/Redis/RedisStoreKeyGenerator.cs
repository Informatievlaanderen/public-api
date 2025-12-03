namespace Public.Api.Infrastructure.Redis
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Common.Infrastructure.Extensions;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Domain;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class RedisStoreKeyGenerator : IStoreKeyGenerator
    {
        private readonly ILogger<RedisStoreKeyGenerator> _logger;

        private const string MunicipalityV2PathPrefix = "/v2/gemeenten/";
        private const string MunicipalityV2CachePrefix = "oslo/municipality:";
        private const string MunicipalityV2CacheKey = MunicipalityV2CachePrefix + "{0}.{1}";
        private static readonly Regex MunicipalityV2Regex = new Regex(@"/v2/gemeenten/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string PostalV2PathPrefix = "/v2/postinfo/";
        private const string PostalV2CachePrefix = "oslo/postalinfo:";
        private const string PostalV2CacheKey = PostalV2CachePrefix + "{0}.{1}";
        private static readonly Regex PostalV2Regex = new Regex(@"/v2/postinfo/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string StreetNameV2PathPrefix = "/v2/straatnamen/";
        private const string StreetNameV2CachePrefix = "oslo/streetname:";
        private const string StreetNameV2CacheKey = StreetNameV2CachePrefix + "{0}.{1}";
        private static readonly Regex StreetNameV2Regex = new Regex(@"/v2/straatnamen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string AddressV2PathPrefix = "/v2/adressen/";
        private const string AddressV2CachePrefix = "oslo/address:";
        private const string AddressV2CacheKey = AddressV2CachePrefix + "{0}.{1}";
        private static readonly Regex AddressV2Regex = new Regex(@"/v2/adressen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string BuildingV2PathPrefix = "/v2/gebouwen/";
        private const string BuildingV2CachePrefix = "oslo/building:";
        private const string BuildingV2CacheKey = BuildingV2CachePrefix + "{0}.{1}";
        private static readonly Regex BuildingV2Regex = new Regex(@"/v2/gebouwen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string BuildingUnitV2PathPrefix = "/v2/gebouweenheden/";
        private const string BuildingUnitV2CachePrefix = "oslo/buildingunit:";
        private const string BuildingUnitV2CacheKey = BuildingUnitV2CachePrefix + "{0}.{1}";
        private static readonly Regex BuildingUnitV2Regex = new Regex(@"/v2/gebouweenheden/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string ParcelV2PathPrefix = "/v2/percelen/";
        private const string ParcelV2CachePrefix = "oslo/parcel:";
        private const string ParcelV2CacheKey = ParcelV2CachePrefix + "{0}.{1}";
        private static readonly Regex ParcelV2Regex = new Regex(@"/v2/percelen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static readonly string[] CachePrefixes =
        {
            MunicipalityV2CachePrefix.ToLowerInvariant(),
            PostalV2CachePrefix.ToLowerInvariant(),
            StreetNameV2CachePrefix.ToLowerInvariant(),
            AddressV2CachePrefix.ToLowerInvariant(),
            BuildingV2CachePrefix.ToLowerInvariant(),
            ParcelV2CachePrefix.ToLowerInvariant(),
        };

        private static readonly IStoreKeyGenerator DefaultStoreKeyGenerator = new DefaultStoreKeyGenerator();

        public RedisStoreKeyGenerator(ILogger<RedisStoreKeyGenerator> logger) => _logger = logger;

        public Task<StoreKey> GenerateStoreKey(StoreKeyContext context)
        {
            var resourcePath = context.HttpRequest.Path.ToString().ToLowerInvariant();

            _logger.LogDebug("Generate store key for '{path}'", resourcePath);

            // http://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
            if (resourcePath.StartsWith(MunicipalityV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, MunicipalityV2Regex, MunicipalityV2CacheKey);
            }

            if (resourcePath.StartsWith(PostalV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, PostalV2Regex, PostalV2CacheKey);
            }

            if (resourcePath.StartsWith(StreetNameV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, StreetNameV2Regex, StreetNameV2CacheKey);
            }
            if (resourcePath.StartsWith(AddressV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, AddressV2Regex, AddressV2CacheKey);
            }

            if (resourcePath.StartsWith(BuildingV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, BuildingV2Regex, BuildingV2CacheKey);
            }

            if (resourcePath.StartsWith(BuildingUnitV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, BuildingUnitV2Regex, BuildingUnitV2CacheKey);
            }

            if (resourcePath.StartsWith(ParcelV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, ParcelV2Regex, ParcelV2CacheKey);
            }

            return DefaultStoreKeyGenerator.GenerateStoreKey(context);
        }

        private Task<StoreKey> GenerateStoreKey(
            StoreKeyContext context,
            string resourcePath,
            Regex regex,
            string cacheKeyFormat)
        {
            var regexResult = regex.Match(resourcePath);

            if (!regexResult.Success)
            {
                return DefaultStoreKeyGenerator.GenerateStoreKey(context);
            }

            var requestHeaders = context.HttpRequest.GetTypedHeaders();
            var acceptType = requestHeaders.DetermineAcceptType(null);

            var cacheKey = string.Format(
                cacheKeyFormat,
                regexResult.Groups["id"],
                acceptType.ToString()).ToLowerInvariant();

            _logger.LogDebug("Generated store key for '{path}' --> '{cacheKey}'", resourcePath, cacheKey);

            return Task.FromResult(new StoreKey { { "cacheKey", cacheKey } });
        }
    }
}
