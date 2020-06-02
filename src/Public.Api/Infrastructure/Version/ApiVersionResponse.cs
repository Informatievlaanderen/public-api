namespace Public.Api.Infrastructure.Version
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ApiVersionResponse
    {
        public ApiVersionResponse(
            MarketingVersion version,
            IDictionary<string, string> components)
        {
            Version = version.ToString();
            Components = components
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        [DataMember(Order = 1)]
        public string Version { get; }

        [DataMember(Order = 2)]
        public IReadOnlyDictionary<string, string> Components { get; }
    }
}
