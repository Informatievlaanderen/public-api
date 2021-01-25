namespace Common.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Extensions;
    using Microsoft.Extensions.Primitives;

    public class BackendResponse
    {
        public string Content { get; private set; }
        public string DownstreamVersion { get; }
        public DateTimeOffset LastModified { get; }
        public string ContentType { get; }
        public bool CameFromCache { get; }
        public HttpStatusCode StatusCode { get; }

        private bool IsXmlContent => ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase);
        private bool IsAtomContent => Content.Contains("<feed xmlns=\"http://www.w3.org/2005/Atom\">", StringComparison.OrdinalIgnoreCase);
        private bool IsProblemDetail => ContentType.Contains("problem", StringComparison.OrdinalIgnoreCase);

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

        public void UpdateNextPageUrlWithQueryParameters(NonPagedQueryCollection requestQuery, string nextPageUrlTemplate)
        {
            if (string.IsNullOrWhiteSpace(nextPageUrlTemplate) || IsProblemDetail)
                return;

            var parameters = requestQuery
                .Where(ParameterIsAllowed)
                .Select(FormatAsUriParameter)
                .Select(EscapeForContentType)
                .Aggregate(string.Empty, (result, parameter) => $"{result}{parameter}");

            if (string.IsNullOrWhiteSpace(parameters))
                return;

            Content = Regex.Replace(
                Content,
                GetNextPagePattern(nextPageUrlTemplate),
                $"$1$2{parameters}$3",
                RegexOptions.IgnoreCase);
        }

        private bool ParameterIsAllowed(KeyValuePair<string, StringValues> parameter)
        {
            // The atom-feed uses 'from' instead 'offset' to get the next set.
            // The new 'from' value is already set in the next-uri and the (previous) 'from' should not be added again.
            if (IsAtomContent)
                return !string.Equals(parameter.Key, "from", StringComparison.OrdinalIgnoreCase);

            return true;
        }

        private static string FormatAsUriParameter(KeyValuePair<string, StringValues> parameter)
            => $"&{parameter.Key}={Uri.EscapeUriString(parameter.Value)}";

        private string GetNextPagePattern(string nextPageUrlTemplate)
        {
            var templateParts = nextPageUrlTemplate
                    .Format(Pattern.NumberPatternPlaceholder, Pattern.NumberPatternPlaceholder)
                    .Split('?');

            if (templateParts.Length > 2)
                throw new ArgumentException($"Argument '{nameof(nextPageUrlTemplate)}' has an invalid format. Multiple '?' where found in '{nextPageUrlTemplate}'");

            var url = new Pattern(templateParts[0], EscapeForContentType);
            var query = new Pattern(templateParts.Length > 1 ? "?" + templateParts[1] : string.Empty, EscapeForContentType);

            if (IsAtomContent)
                return $"(<link href=\"{url})({query})(\" rel=\"next\" />)";

            if (IsXmlContent)
                return $"(<volgende>{url})({query})(</volgende>)";

            return $"(\"volgende\"\\s*:\\s*\"{url})({query})(\")";
        }

        private string EscapeForContentType(string value) => IsXmlContent ? new XText(value).ToString() : value;

        private class Pattern
        {
            public const string NumberPatternPlaceholder = "__NUMBER_PATTERN_PLACEHOLDER_THAT_WONT_BE_REGEX_ESCAPED__";

            private readonly Func<string, string> _escapeForContentType;
            private readonly string _escapedPattern;

            public Pattern(string patternValue, Func<string, string> escapeForContentType)
            {
                _escapeForContentType = escapeForContentType ?? throw new ArgumentNullException(nameof(escapeForContentType));
                _escapedPattern = BuildPattern(patternValue);
            }

            private static string BuildPattern(string patternValue)
            {
                return patternValue.IsNullOrWhiteSpace()
                    ? string.Empty
                    : Regex
                        .Escape(patternValue)
                        .Replace(NumberPatternPlaceholder, @"\d+");
            }

            public override string ToString() => _escapeForContentType(_escapedPattern);
        }
    }
}
