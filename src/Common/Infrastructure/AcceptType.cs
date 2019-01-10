namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Http.Headers;

    public enum AcceptType
    {
        Json,
        JsonLd,
        Xml,
        Atom
    }

    public static class AcceptTypes
    {
        public const string Json = "application/json";
        public const string JsonLd = "application/ld+json";
        public const string Xml = "application/xml";
        public const string Atom = "application/atom+xml";
    }

    public static class AcceptTypeExtensions
    {
        public static string ToMimeTypeString(this AcceptType acceptType)
        {
            switch (acceptType)
            {
                case AcceptType.Json:
                    return AcceptTypes.Json;

                case AcceptType.JsonLd:
                    return AcceptTypes.JsonLd;

                case AcceptType.Xml:
                    return AcceptTypes.Xml;

                case AcceptType.Atom:
                    return AcceptTypes.Atom;

                default:
                    throw new ArgumentOutOfRangeException(nameof(acceptType), acceptType, null);
            }
        }

        public static AcceptType DetermineAcceptType(this RequestHeaders requestHeaders)
        {
            var headersByQuality = requestHeaders.Accept.OrderBy(x => x.Quality);

            foreach (var headerWithQuality in headersByQuality)
            {
                if (!headerWithQuality.MediaType.HasValue)
                    continue;

                if (headerWithQuality.MediaType.Value.ToLowerInvariant().Contains("atom".ToLowerInvariant()))
                    return AcceptType.Atom;

                if (headerWithQuality.MediaType.Value.ToLowerInvariant().Contains("xml".ToLowerInvariant()))
                    return AcceptType.Xml;

                if (headerWithQuality.MediaType.Value.ToLowerInvariant().Contains("ld+json".ToLowerInvariant()))
                    return AcceptType.JsonLd;

                if (headerWithQuality.MediaType.Value.ToLowerInvariant().Contains("json".ToLowerInvariant()))
                    return AcceptType.Json;
            }

            return AcceptType.Json;
        }
    }
}
