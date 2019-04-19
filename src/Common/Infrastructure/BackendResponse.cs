namespace Common.Infrastructure
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public class BackendResponse
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool CameFromCache { get; set; }

        public BackendResponse(string content, string contentType, bool cameFromCache)
        {
            Content = content;
            ContentType = contentType;
            CameFromCache = cameFromCache;
        }

        public void UpdateNextPageUrlWithQueryParameters(NonPagedQueryCollection requestQuery, string nextPageUrlOption)
        {
            if (string.IsNullOrWhiteSpace(nextPageUrlOption) || requestQuery.IsEmpty)
                return;

            var parameters = requestQuery
                .Aggregate("", (result, filterQueryParameter) => $"{result}&{filterQueryParameter.Key}={filterQueryParameter.Value}");

            Content = Regex.Replace(
                Content,
                GetNextPagePattern(nextPageUrlOption),
                $"$1{EscapeForContentType(parameters)}$2",
                RegexOptions.IgnoreCase);
        }

        private bool IsXmlContent => ContentType == AcceptTypes.Xml || ContentType == AcceptTypes.Atom;

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

        private string EscapeForContentType(string value) => IsXmlContent ? new System.Xml.Linq.XText(value).ToString() : value;

    }
}
