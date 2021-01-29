namespace Common.Infrastructure.Extensions
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;
    using Newtonsoft.Json;
    using RestSharp;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public static class RestResponseExtensions
    {
        public static ProblemDetails GetProblemDetails(this IRestResponse response)
            => response.ContentType.Contains("xml", StringComparison.InvariantCultureIgnoreCase)
                ? DataContractDeserializeXlmResponse<ProblemDetails>(response)
                : JsonConvert.DeserializeObject<ProblemDetails>(response.Content);

        private static T DataContractDeserializeXlmResponse<T>(IRestResponse restResponse) where T : class, new()
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
    }
}
