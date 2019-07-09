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
        public const string Json = MediaTypeNames.Application.Json;
        public const string JsonLd = "application/ld+json";
        public const string Xml = MediaTypeNames.Application.Xml;
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

        public static AcceptType DetermineAcceptType(this RequestHeaders requestHeaders, string format)
            => Enum.TryParse(format, ignoreCase: true, out AcceptType acceptType)
                ? acceptType
                : requestHeaders.DetermineAcceptType();

        public static AcceptType DetermineAcceptType(this RequestHeaders requestHeaders)
        {
            var headersByQuality = requestHeaders
                .Accept
                .OrderBy(header => header.Quality)
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
            }

            throw new ApiException("Accept type is not acceptable", StatusCodes.Status406NotAcceptable);
        }

        private static bool Contains(this MediaTypeHeaderValue headerValue, string mineType)
            => headerValue.MediaType.Value.Contains(mineType, StringComparison.InvariantCultureIgnoreCase);
    }
}
