namespace Common.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using RestSharp;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public static class RestResponseExtensions
    {
        public static ProblemDetails GetProblemDetails(this RestResponse response)
            => response.ContentType.Contains("xml", StringComparison.InvariantCultureIgnoreCase)
                ? DataContractDeserializeXlmResponse<ProblemDetails>(response)
                : JsonConvert.DeserializeObject<ProblemDetails>(response.Content);

        private static T DataContractDeserializeXlmResponse<T>(RestResponse restResponse) where T : class, new()
        {
            try
            {
                var xmlReader = XmlDictionaryReader.CreateTextReader(restResponse.RawBytes, new XmlDictionaryReaderQuotas());
                var serializer = new DataContractSerializer(typeof(T));
                return (T?)serializer.ReadObject(xmlReader) ?? new T();
            }
            catch (Exception)
            {
                return new T();
            }
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> HeadersToKeyValuePairs(this RestResponse restResponse)
        {
            if(restResponse.Headers is null)
                yield break;

            var responseHeadersByName = restResponse
                .Headers
                .GroupBy(h => h.Name)
                .ToDictionary(x => x.Key, y => y.Select(p => p.Value).ToList());

            foreach (var header in responseHeadersByName)
            {
                if(header.Value.Count == 1)
                    yield return new KeyValuePair<string, StringValues>(header.Key, new StringValues(header.Value.First()));
                else
                    yield return new KeyValuePair<string, StringValues>(header.Key, new StringValues(header.Value.ToArray()));
            }
        }
    }
}
