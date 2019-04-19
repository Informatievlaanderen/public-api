namespace Common.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Newtonsoft.Json;
    using RestSharp;
    using System;
    using System.Linq;

    public static class RestRequestHelpers
    {
        private const int DefaultLimit = 100;
        private const int MaximumLimit = 10000;

        public static IRestRequest AddSorting(
            this IRestRequest request,
            string sort)
        {
            if (!string.IsNullOrWhiteSpace(sort))
                request.AddHeader(AddSortingExtension.HeaderName, sort.CreateSortObject());

            return request;
        }

        public static IRestRequest AddPagination(
            this IRestRequest request,
            int? offset,
            int? limit)
        {
            offset = offset ?? 0;
            limit = limit ?? DefaultLimit;

            if (offset <= 0)
                offset = 0;

            if (limit > MaximumLimit)
                limit = MaximumLimit;

            request.AddHeader(AddPaginationExtension.HeaderName, $"{offset},{limit}");

            return request;
        }

        public static IRestRequest AddFiltering(
            this IRestRequest request,
            object filter)
        {
            if (filter != null)
                request.AddHeader(ExtractFilteringRequestExtension.HeaderName, JsonConvert.SerializeObject(filter));

            return request;
        }

        private static string CreateSortObject(this string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return string.Empty;

            var sortPieces = sort.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sortField = sortPieces.First().ToLowerInvariant();

            return sortField.StartsWith("-")
                ? $"descending,{sortField.Substring(1)}"
                : $"ascending,{sortField}";
        }
    }
}
