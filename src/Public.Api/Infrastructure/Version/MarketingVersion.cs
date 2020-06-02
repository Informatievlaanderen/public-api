namespace Public.Api.Infrastructure.Version
{
    using Microsoft.Extensions.Configuration;

    public class MarketingVersion
    {
        private readonly string _version;

        public MarketingVersion(IConfiguration configuration)
        {
            _version = configuration["ApiMarketingVersion"];
        }

        public override string ToString()
        {
            return _version;
        }
    }
}
