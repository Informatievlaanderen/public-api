namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class NonPagedQueryCollection : QueryCollection
    {
        public bool IsEmpty => Count == 0;

        public NonPagedQueryCollection(IEnumerable<KeyValuePair<string, StringValues>> store)
            : base(Filter(store)) { }

        private static readonly IEnumerable<string> PageQueryParameters = new[] {"limit", "offset"};

        private static Dictionary<string, StringValues> Filter(IEnumerable<KeyValuePair<string, StringValues>> query)
        {
            if (query == null)
                return new Dictionary<string, StringValues>();

            bool IsNotPageParameter(KeyValuePair<string, StringValues> keyValuePair)
                => PageQueryParameters.Contains(keyValuePair.Key, StringComparer.InvariantCultureIgnoreCase) == false;

            return query
                .Where(queryParameter => queryParameter.Value.Count > 0)
                .Where(IsNotPageParameter)
                .ToDictionary(queryParameter => queryParameter.Key, queryParameter => queryParameter.Value);
        }
    }
}
