namespace Public.Api.Infrastructure
{
    using System;
    using System.Text.RegularExpressions;

    public static class UrlExtensions
    {
        public static Url Combine(this Url url, params string[] parts)
        {
            parts = PushToArray(url.ToString(), parts);

            return new Url(CombineParts(parts));
        }

        /// <summary>
        /// Github.com/Flurl
        /// Basically a Path.Combine for URLs. Ensures exactly one '/' separates each segment,
        /// and exactly on '&amp;' separates each query parameter.
        /// URL-encodes illegal characters but not reserved characters.
        /// </summary>
        /// <param name="parts">URL parts to combine.</param>
        private static string CombineParts(params string[] parts)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            string result = "";
            bool inQuery = false, inFragment = false;

            string CombineEnsureSingleSeparator(string a, string b, char separator)
            {
                if (string.IsNullOrEmpty(a))
                {
                    return b;
                }

                if (string.IsNullOrEmpty(b))
                {
                    return a;
                }

                return a.TrimEnd(separator) + separator + b.TrimStart(separator);
            }

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                if (result.OrdinalEndsWith("?") || part.OrdinalStartsWith("?"))
                {
                    result = CombineEnsureSingleSeparator(result, part, '?');
                }
                else if (result.OrdinalEndsWith("#") || part.OrdinalStartsWith("#"))
                {
                    result = CombineEnsureSingleSeparator(result, part, '#');
                }
                else if (inFragment)
                {
                    result += part;
                }
                else if (inQuery)
                {
                    result = CombineEnsureSingleSeparator(result, part, '&');
                }
                else
                {
                    result = CombineEnsureSingleSeparator(result, part, '/');
                }

                if (part.OrdinalContains("#"))
                {
                    inQuery = false;
                    inFragment = true;
                }
                else if (!inFragment && part.OrdinalContains("?"))
                {
                    inQuery = true;
                }
            }

            return EncodeIllegalCharacters(result);
        }

        /// <summary>
        /// URL-encodes characters in a string that are neither reserved nor unreserved. Avoids encoding reserved characters such as '/' and '?'. Avoids encoding '%' if it begins a %-hex-hex sequence (i.e. avoids double-encoding).
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
        /// <returns>The encoded URL.</returns>
        public static string EncodeIllegalCharacters(string s, bool encodeSpaceAsPlus = false)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (encodeSpaceAsPlus)
            {
                s = s.Replace(" ", "+");
            }

            // Uri.EscapeUriString mostly does what we want - encodes illegal characters only - but it has a quirk
            // in that % isn't illegal if it's the start of a %-encoded sequence https://stackoverflow.com/a/47636037/62600

            // no % characters, so avoid the regex overhead
            if (!s.OrdinalContains("%"))
            {
                return Uri.EscapeUriString(s);
            }

            // pick out all %-hex-hex matches and avoid double-encoding
            return Regex.Replace(s, "(.*?)((%[0-9A-Fa-f]{2})|$)", c => {
                var a = c.Groups[1].Value; // group 1 is a sequence with no %-encoding - encode illegal characters
                var b = c.Groups[2].Value; // group 2 is a valid 3-character %-encoded sequence - leave it alone!
                return Uri.EscapeUriString(a) + b;
            });
        }

        private static string[] PushToArray(string s, string[] a2)
        {
            var a1 = new string[a2.Length + 1];
            a1[0] = s;

            for (var i = 1; i < a2.Length + 1; i++)
            {
                a1[i] = a2[i - 1];
            }

            return a1;
        }
    }

    internal static class StringExtensions
    {
        internal static bool OrdinalContains(this string? s, string value, bool ignoreCase = false) =>
            s != null && s.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;

        internal static bool OrdinalStartsWith(this string? s, string value, bool ignoreCase = false) =>
            s != null && s.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        internal static bool OrdinalEndsWith(this string? s, string value, bool ignoreCase = false) =>
            s != null && s.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
}
