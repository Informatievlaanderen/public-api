namespace Public.Api.Infrastructure
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Domain;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class RedisStoreKeyGenerator : IStoreKeyGenerator
    {
        private const string MunicipalityCacheKey = "legacy/municipality:{0}.{1}";
        private static readonly Regex MunicipalityRegex = new Regex(@"/v1/gemeenten/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string PostalCacheKey = "legacy/postalinfo:{0}.{1}";
        private static readonly Regex PostalRegex = new Regex(@"/v1/postinfo/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string StreetNameCacheKey = "legacy/streetname:{0}.{1}";
        private static readonly Regex StreetNameRegex = new Regex(@"/v1/straatnamen/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string AddressCacheKey = "legacy/address:{0}.{1}";
        private static readonly Regex AddressRegex = new Regex(@"/v1/adressen/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string ParcelCacheKey = "legacy/parcel:{0}.{1}";
        private static readonly Regex ParcelRegex = new Regex(@"/v1/percelen/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // TODO: Add the others

        private const string BuildingCacheKey = "legacy/building:{0}.{1}";
        private static readonly Regex BuildingRegex = new Regex(@"/v1/gebouwen/(?<id>\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly IStoreKeyGenerator DefaultStoreKeyGenerator = new DefaultStoreKeyGenerator();

        public Task<StoreKey> GenerateStoreKey(StoreKeyContext context)
        {
            var resourcePath = context.HttpRequest.Path.ToString().ToLowerInvariant();

            // http://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
            if (resourcePath.StartsWith("/v1/gemeenten/"))
                return GenerateStoreKey(context, resourcePath, MunicipalityRegex, MunicipalityCacheKey);

            if (resourcePath.StartsWith("/v1/postinfo/"))
                return GenerateStoreKey(context, resourcePath, PostalRegex, PostalCacheKey);

            if (resourcePath.StartsWith("/v1/straatnamen/"))
                return GenerateStoreKey(context, resourcePath, StreetNameRegex, StreetNameCacheKey);

            if (resourcePath.StartsWith("/v1/adressen/"))
                return GenerateStoreKey(context, resourcePath, AddressRegex, AddressCacheKey);

            if (resourcePath.StartsWith("/v1/percelen/"))
                return GenerateStoreKey(context, resourcePath, ParcelRegex, ParcelCacheKey);

            // TODO: Add the others

            if (resourcePath.StartsWith("/v1/gebouwen/"))
                return GenerateStoreKey(context, resourcePath, BuildingRegex, BuildingCacheKey);

            return DefaultStoreKeyGenerator.GenerateStoreKey(context);
        }

        private static Task<StoreKey> GenerateStoreKey(
            StoreKeyContext context,
            string resourcePath,
            Regex regex,
            string cacheKeyFormat)
        {
            var regexResult = regex.Match(resourcePath);

            if (!regexResult.Success)
                return DefaultStoreKeyGenerator.GenerateStoreKey(context);

            var requestHeaders = context.HttpRequest.GetTypedHeaders();
            var acceptType = requestHeaders.DetermineAcceptType();

            return Task.FromResult(new StoreKey
            {
                { "cacheKey", string.Format(cacheKeyFormat, regexResult.Groups["id"], acceptType.ToString()).ToLowerInvariant() }
            });
        }
    }
}
