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

            const string numberPatternPlaceholder = "__NUMBER_PATTERN_PLACEHOLDER_THAT_WONT_BE_REGEX_ESCAPED__";
            var nextPageUrlOptionWithPlaceholders = string.Format(nextPageUrlOption, numberPatternPlaceholder, numberPatternPlaceholder);
            var nextPageUrlValePattern = Regex
                .Escape(nextPageUrlOptionWithPlaceholders)
                .Replace(numberPatternPlaceholder, "\\d+");

            var parameters = requestQuery
                .Aggregate("", (result, filterQueryParameter) => $"{result}&{filterQueryParameter.Key}={filterQueryParameter.Value}");

            var isXml = ContentType.ToLower().Contains("xml");
            var replaceData = new {
                Pattern = isXml
                    ? GetXmlPatternFor(nextPageUrlValePattern)
                    : $"(\"volgende\"\\s*:\\s*\"{nextPageUrlValePattern})(\")",
                Replacement = isXml
                    ? $"$1{Xml.Escape(parameters)}$2"
                    : $"$1{parameters}$2"
            };

            Content = Regex.Replace(
                Content,
                replaceData.Pattern,
                replaceData.Replacement,
                RegexOptions.IgnoreCase);
        }

        private string GetXmlPatternFor(string nextPageUrlValePattern)
        {
            return Regex.IsMatch(Content, "<feed xmlns=\"http://www.w3.org/2005/Atom\">")
                ? $"(<link href=\"{Xml.Escape(nextPageUrlValePattern)})(\" rel=\"next\" />)"
                : $"(<volgende>{Xml.Escape(nextPageUrlValePattern)})(</volgende>)";
        }

        private static class Xml
        {
            public static string Escape(string str) => new System.Xml.Linq.XText(str).ToString();
        }
    }
}
