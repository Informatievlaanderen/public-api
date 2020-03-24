namespace Common.Infrastructure
{
    using Microsoft.AspNetCore.Http.Headers;
    using System;
    using System.Linq;
    using Microsoft.Net.Http.Headers;
    using System.Net.Mime;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;

    public enum AcceptType
    {
        Json,
        JsonLd,
        Xml,
        Atom
    }

    public static class AcceptTypes
    {
        public const string Any = "*/*";
        public const string Json = MediaTypeNames.Application.Json;
        public const string JsonLd = "application/ld+json";
        public const string JsonProblem = "application/problem+json";
        public const string Xml = MediaTypeNames.Application.Xml;
        public const string Atom = "application/atom+xml";
        public const string XmlProblem = "application/problem+xml";
    }

    public static class AcceptTypeExtensions
    {
        public static string ToMimeTypeString(this AcceptType acceptType)
        {
            return acceptType switch
            {
                AcceptType.Json => AcceptTypes.Json,
                AcceptType.JsonLd => AcceptTypes.JsonLd,
                AcceptType.Xml => AcceptTypes.Xml,
                AcceptType.Atom => AcceptTypes.Atom,
                _ => throw new ArgumentOutOfRangeException(nameof(acceptType), acceptType, null)
            };
        }

        public static string ToProblemResponseMimeTypeString(this AcceptType acceptType)
        {
            return acceptType switch
            {
                AcceptType.Json => AcceptTypes.JsonProblem,
                AcceptType.JsonLd => AcceptTypes.JsonProblem,
                AcceptType.Xml => AcceptTypes.XmlProblem,
                AcceptType.Atom => AcceptTypes.XmlProblem,
                _ => throw new ArgumentOutOfRangeException(nameof(acceptType), acceptType, null)
            };
        }

        public static AcceptType DetermineAcceptType(this RequestHeaders requestHeaders, string format)
            => Enum.TryParse(format, ignoreCase: true, out AcceptType acceptType)
                ? acceptType
                : requestHeaders.DetermineAcceptType();

        public static AcceptType DetermineAcceptType(this RequestHeaders requestHeaders)
        {
            var acceptHeaders = requestHeaders.Accept;

            if (acceptHeaders == null || acceptHeaders.Count == 0)
                return AcceptType.Json;

            var headersByQuality = acceptHeaders
                .OrderByDescending(header => header, MediaTypeHeaderValueComparer.QualityComparer)
                .Where(header => header.MediaType.HasValue);

            foreach (var headerValue in headersByQuality)
            {
                if (headerValue.Contains(AcceptTypes.Atom))
                    return AcceptType.Atom;

                if (headerValue.Contains(AcceptTypes.Xml))
                    return AcceptType.Xml;

                if (headerValue.Contains(AcceptTypes.JsonLd))
                    return AcceptType.JsonLd;

                if (headerValue.Contains(AcceptTypes.Json))
                    return AcceptType.Json;

                if (headerValue.Contains(AcceptTypes.Any))
                    return AcceptType.Json;
            }

            throw new ApiException("Ongeldig formaat.", StatusCodes.Status406NotAcceptable);
        }

        private static bool Contains(this MediaTypeHeaderValue headerValue, string mineType)
            => headerValue.MediaType.Value.Contains(mineType, StringComparison.InvariantCultureIgnoreCase);
    }
}
