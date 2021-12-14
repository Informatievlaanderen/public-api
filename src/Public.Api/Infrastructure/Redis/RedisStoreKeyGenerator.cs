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

        private const string MunicipalityPathPrefix = "/v1/gemeenten/";
        private const string MunicipalityCachePrefix = "legacy/municipality:";
        private const string MunicipalityCacheKey = MunicipalityCachePrefix + "{0}.{1}";
        private static readonly Regex MunicipalityRegex = new Regex(@"/v1/gemeenten/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string MunicipalityV2PathPrefix = "/v2/gemeenten/";
        private const string MunicipalityV2CachePrefix = "oslo/municipality:";
        private const string MunicipalityV2CacheKey = MunicipalityV2CachePrefix + "{0}.{1}";
        private static readonly Regex MunicipalityV2Regex = new Regex(@"/v2/gemeenten/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string PostalPathPrefix = "/v1/postinfo/";
        private const string PostalCachePrefix = "legacy/postalinfo:";
        private const string PostalCacheKey = PostalCachePrefix + "{0}.{1}";
        private static readonly Regex PostalRegex = new Regex(@"/v1/postinfo/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string PostalV2PathPrefix = "/v2/postinfo/";
        private const string PostalV2CachePrefix = "oslo/postalinfo:";
        private const string PostalV2CacheKey = PostalV2CachePrefix + "{0}.{1}";
        private static readonly Regex PostalV2Regex = new Regex(@"/v2/postinfo/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string StreetNamePathPrefix = "/v1/straatnamen/";
        private const string StreetNameCachePrefix = "legacy/streetname:";
        private const string StreetNameCacheKey = StreetNameCachePrefix + "{0}.{1}";
        private static readonly Regex StreetNameRegex = new Regex(@"/v1/straatnamen/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string StreetNameV2PathPrefix = "/v2/straatnamen/";
        private const string StreetNameV2CachePrefix = "oslo/streetname:";
        private const string StreetNameV2CacheKey = StreetNameV2CachePrefix + "{0}.{1}";
        private static readonly Regex StreetNameV2Regex = new Regex(@"/v2/straatnamen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string AddressPathPrefix = "/v1/adressen/";
        private const string AddressCachePrefix = "legacy/address:";
        private const string AddressCacheKey = AddressCachePrefix + "{0}.{1}";
        private static readonly Regex AddressRegex = new Regex(@"/v1/adressen/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string AddressV2PathPrefix = "/v2/adressen/";
        private const string AddressV2CachePrefix = "oslo/address:";
        private const string AddressV2CacheKey = AddressV2CachePrefix + "{0}.{1}";
        private static readonly Regex AddressV2Regex = new Regex(@"/v2/adressen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // As long as we do not control WFS, buildings cannot be cached
        //private const string BuildingPathPrefix = "/v1/gebouwen/";
        //private const string BuildingCachePrefix = "legacy/building:";
        //private const string BuildingCacheKey = BuildingCachePrefix + "{0}.{1}";
        //private static readonly Regex BuildingRegex = new Regex(@"/v1/gebouwen/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string BuildingUnitPathPrefix = "/v1/gebouweenheden/";
        private const string BuildingUnitCachePrefix = "legacy/buildingunit:";
        private const string BuildingUnitCacheKey = BuildingUnitCachePrefix + "{0}.{1}";
        private static readonly Regex BuildingUnitRegex = new Regex(@"/v1/gebouweenheden/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string BuildingUnitV2PathPrefix = "/v2/gebouweenheden/";
        private const string BuildingUnitV2CachePrefix = "oslo/buildingunit:";
        private const string BuildingUnitV2CacheKey = BuildingUnitCachePrefix + "{0}.{1}";
        private static readonly Regex BuildingUnitV2Regex = new Regex(@"/v2/gebouweenheden/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string ParcelPathPrefix = "/v1/percelen/";
        private const string ParcelCachePrefix = "legacy/parcel:";
        private const string ParcelCacheKey = ParcelCachePrefix + "{0}.{1}";
        private static readonly Regex ParcelRegex = new Regex(@"/v1/percelen/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string ParcelV2PathPrefix = "/v2/percelen/";
        private const string ParcelV2CachePrefix = "oslo/parcel:";
        private const string ParcelV2CacheKey = ParcelV2CachePrefix + "{0}.{1}";
        private static readonly Regex ParcelV2Regex = new Regex(@"/v2/percelen/(?<id>\d*)(?<format>\.(jsonld))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string PublicServicePathPrefix = "/v1/dienstverleningen/";
        private const string PublicServiceCachePrefix = "legacy/publicservice:";
        private const string PublicServiceCacheKey = PublicServiceCachePrefix + "{0}.{1}";
        private static readonly Regex PublicServiceRegex = new Regex(@"/v1/dienstverleningen/(?<id>\d*)(?<format>\.(json|xml))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // TODO: Add organisations when the time comes

        public static readonly string[] CachePrefixes =
        {
            MunicipalityCachePrefix.ToLowerInvariant(),
            MunicipalityV2CachePrefix.ToLowerInvariant(),
            PostalCachePrefix.ToLowerInvariant(),
            PostalV2CachePrefix.ToLowerInvariant(),
            StreetNameCachePrefix.ToLowerInvariant(),
            StreetNameV2CachePrefix.ToLowerInvariant(),
            AddressCachePrefix.ToLowerInvariant(),
            AddressV2CachePrefix.ToLowerInvariant(),
            //BuildingCachePrefix.ToLowerInvariant(),
            BuildingUnitCachePrefix.ToLowerInvariant(),
            ParcelCachePrefix.ToLowerInvariant(),
            ParcelV2CachePrefix.ToLowerInvariant(),
            PublicServiceCachePrefix.ToLowerInvariant()
        };

        private static readonly IStoreKeyGenerator DefaultStoreKeyGenerator = new DefaultStoreKeyGenerator();

        public RedisStoreKeyGenerator(ILogger<RedisStoreKeyGenerator> logger) => _logger = logger;

        public Task<StoreKey> GenerateStoreKey(StoreKeyContext context)
        {
            var resourcePath = context.HttpRequest.Path.ToString().ToLowerInvariant();

            _logger.LogDebug("Generate store key for '{path}'", resourcePath);

            // http://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
            if (resourcePath.StartsWith(MunicipalityPathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, MunicipalityRegex, MunicipalityCacheKey);
            }

            if (resourcePath.StartsWith(MunicipalityV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, MunicipalityV2Regex, MunicipalityV2CacheKey);
            }

            if (resourcePath.StartsWith(PostalPathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, PostalRegex, PostalCacheKey);
            }

            if (resourcePath.StartsWith(PostalV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, PostalV2Regex, PostalV2CacheKey);
            }

            if (resourcePath.StartsWith(StreetNamePathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, StreetNameRegex, StreetNameCacheKey);
            }

            if (resourcePath.StartsWith(StreetNameV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, StreetNameV2Regex, StreetNameV2CacheKey);
            }

            if (resourcePath.StartsWith(AddressPathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, AddressRegex, AddressCacheKey);
            }

            if (resourcePath.StartsWith(AddressV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, AddressV2Regex, AddressV2CacheKey);
            }

            // As long as we do not control WFS, buildings cannot be cached
            //if (resourcePath.StartsWith(BuildingPathPrefix))
            //{
            //    return GenerateStoreKey(context, resourcePath, BuildingRegex, BuildingCacheKey);
            //}

            if (resourcePath.StartsWith(BuildingUnitPathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, BuildingUnitRegex, BuildingUnitCacheKey);
            }

            if (resourcePath.StartsWith(ParcelPathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, ParcelRegex, ParcelCacheKey);
            }

            if (resourcePath.StartsWith(ParcelV2PathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, ParcelV2Regex, ParcelV2CacheKey);
            }

            if (resourcePath.StartsWith(PublicServicePathPrefix))
            {
                return GenerateStoreKey(context, resourcePath, PublicServiceRegex, PublicServiceCacheKey);
            }

            // TODO: Add organisations when the time comes

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
