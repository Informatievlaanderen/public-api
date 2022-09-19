namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    public class ListResponse<T> : Dictionary<string, T>
    {
        public static ListResponse<T> From(IEnumerable<KeyValuePair<string, T>>? collection)
        {
            if (collection is null)
            {
                return new ListResponse<T>();
            }

            var listResponse = new ListResponse<T>();
            foreach (var (registry, status) in collection.OrderBy(pair => pair.Key))
            {
                listResponse[registry] = status;
            }

            return listResponse;
        }

        public ListResponse()
        { }

        protected ListResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
