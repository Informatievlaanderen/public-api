namespace Common.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Mvc.Formatters;

    public static class EndPointTypeExtensions
    {
        public static IEnumerable<AcceptType> GetAcceptedTypes(this EndpointType endpointType)
        {
            return endpointType switch
            {
                EndpointType.Legacy
                => new[]
                {
                    AcceptType.Json,
                    AcceptType.Xml
                },
                EndpointType.Sync
                => new[]
                {
                    AcceptType.Atom,
                    AcceptType.Xml
                },
                EndpointType.BackOffice
                    => new []
                    {
                        AcceptType.Json
                    },
                EndpointType.Oslo
                    => new []
                    {
                        AcceptType.JsonLd
                    },
                _ => new[] { AcceptType.Json }
            };
        }

        public static IEnumerable<AcceptType> GetContentTypes(this EndpointType endpointType)
        {
            return endpointType switch
            {
                EndpointType.BackOffice
                    => new []
                    {
                        AcceptType.Json
                    },
                EndpointType.Oslo
                    => new[]
                    {
                        AcceptType.JsonLd
                    },
                _ => new[] { AcceptType.Json }
            };
        }

        public static MediaTypeCollection Produces(this EndpointType endpointType)
        {
            return endpointType
                .GetAcceptedTypes()
                .SelectMany(type =>
                    new []
                    {
                        AcceptTypeExtensions.ToMimeTypeString(type),
                        AcceptTypeExtensions.ToProblemResponseMimeTypeString(type)
                    })
                .Distinct()
                .OrderBy(type => type, new AcceptTypeComparer())
                .Aggregate(
                    new MediaTypeCollection(),
                    (collection, type) =>
                    {
                        collection.Add(type);
                        return collection;
                    });
        }

        public static MediaTypeCollection Consumes(this EndpointType endpointType)
        {
            return endpointType
                .GetContentTypes()
                .SelectMany(type =>
                    new []
                    {
                        AcceptTypeExtensions.ToMimeTypeString(type)
                    })
                .Distinct()
                .OrderBy(type => type, new AcceptTypeComparer())
                .Aggregate(
                    new MediaTypeCollection(),
                    (collection, type) =>
                    {
                        collection.Add(type);
                        return collection;
                    });
        }

        private class AcceptTypeComparer : IComparer<string>
        {
            public int Compare([AllowNull] string x, [AllowNull] string y)
            {
                if (x == null || y == null)
                    return DefaultCompare(x, y);

                var problemResult = 0;
                if (ContainsProblem(x))
                    problemResult -= 1;
                if (ContainsProblem(y))
                    problemResult += 1;

                return problemResult != 0 ? problemResult : string.CompareOrdinal(x, y);
            }

            private static int DefaultCompare([AllowNull] string x, [AllowNull] string y)
                => string.CompareOrdinal(x, y);

            private static bool ContainsProblem(string value) =>
                value.Contains("problem+", StringComparison.OrdinalIgnoreCase);
        }
    }
}
