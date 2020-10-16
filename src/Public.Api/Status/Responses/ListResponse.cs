namespace Public.Api.Status.Responses
{
    using System.Collections.Generic;
    using System.Linq;

    public class ListResponse<T> : Dictionary<string, IEnumerable<T>>
    {
        public static ListResponse<T> From(IEnumerable<KeyValuePair<string, IEnumerable<T>>> collection)
        {
            if (collection == null)
                return new ListResponse<T>();

            var listResponse = new ListResponse<T>();
            foreach (var (registry, status) in collection.OrderBy(pair => pair.Key))
                listResponse[registry] = status;

            return listResponse;
        }
    }
}
