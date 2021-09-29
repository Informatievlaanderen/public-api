namespace Common.Infrastructure.Extensions
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        public static ProblemDetails GetProblemDetails(this HttpResponseMessage response, string responseContent)
        {
            if (response.Content.Headers.ContentType.MediaType.Contains("xml", StringComparison.InvariantCultureIgnoreCase))
                return DataContractDeserializeXlmResponse<ProblemDetails>(responseContent);
            else
                return JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        }

        private static T DataContractDeserializeXlmResponse<T>(string responseContent) where T : class, new()
        {
            try
            {
                using var stringReader = new StringReader(responseContent);
                using var xmlReader = XmlReader.Create(stringReader);
                var serializer = new DataContractSerializer(typeof(T));
                return (T?) serializer.ReadObject(xmlReader) ?? new T();
            }
            catch (Exception)
            {
                return new T();
            }
        }
    }
}
