namespace Public.Api.Infrastructure
{
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Http;

    public class BackendListResponseResult : BackendResponseResult
    {
        private BackendListResponseResult(BackendResponse response)
            : base(response) { }

        public static BackendListResponseResult Create(
            BackendResponse response,
            IQueryCollection requestQuery,
            string nextPageUrlOption)
        {
            var nonPagedQueryCollection = new NonPagedQueryCollection(requestQuery);

            response.UpdateNextPageUrlWithQueryParameters(nonPagedQueryCollection, nextPageUrlOption);

            return new BackendListResponseResult(response);
        }
    }
}
