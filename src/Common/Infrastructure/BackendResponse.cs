namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class BackendResponse
    {
        public string Content { get; private set; }
        public string DownstreamVersion { get; }
        public DateTimeOffset LastModified { get; }
        public string ContentType { get; }
        public bool CameFromCache { get; }
        public HttpStatusCode StatusCode { get; }

        private bool IsXmlContent => ContentType == AcceptTypes.Xml || ContentType == AcceptTypes.Atom;

        public BackendResponse(
            string content,
            string downstreamVersion,
            DateTimeOffset lastModified,
            string contentType,
            bool cameFromCache,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Content = content;
            DownstreamVersion = downstreamVersion;
            LastModified = lastModified;
            ContentType = contentType.ToLowerInvariant();
            CameFromCache = cameFromCache;
            StatusCode = statusCode;
        }

        public void UpdateNextPageUrlWithQueryParameters(NonPagedQueryCollection requestQuery, string nextPageUrlOption)
        {
            if (string.IsNullOrWhiteSpace(nextPageUrlOption) || requestQuery.IsEmpty)
                return;

            var parameters = requestQuery
                .Aggregate(string.Empty, (result, filterQueryParameter) => $"{result}&{filterQueryParameter.Key}={Uri.EscapeUriString(filterQueryParameter.Value)}");

            Content = Regex.Replace(
                Content,
                GetNextPagePattern(nextPageUrlOption),
                $"$1{EscapeForContentType(parameters)}$2",
                RegexOptions.IgnoreCase);
        }

        private string GetNextPagePattern(string nextPageUrlOption)
        {
            const string numberPatternPlaceholder = "__NUMBER_PATTERN_PLACEHOLDER_THAT_WONT_BE_REGEX_ESCAPED__";
            var nextPageUrlOptionWithPlaceholders = string.Format(nextPageUrlOption, numberPatternPlaceholder, numberPatternPlaceholder);
            var nextPageUrlValePattern = Regex
                .Escape(nextPageUrlOptionWithPlaceholders)
                .Replace(numberPatternPlaceholder, "\\d+");

            var escapedValuePattern = EscapeForContentType(nextPageUrlValePattern);

            if (IsXmlContent)
                return Content.Contains("<feed xmlns=\"http://www.w3.org/2005/Atom\">")
                    ? $"(<link href=\"{escapedValuePattern})(\" rel=\"next\" />)"
                    : $"(<volgende>{escapedValuePattern})(</volgende>)";

            return $"(\"volgende\"\\s*:\\s*\"{escapedValuePattern})(\")";
        }

        private string EscapeForContentType(string value) => IsXmlContent ? new XText(value).ToString() : value;
    }
}
