namespace Common.Infrastructure
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using RestSharp.Serializers;
    using RestSharp;

    public class JsonNetSerializer : IRestSerializer, ISerializer, IDeserializer
    {
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);

        public string Serialize(Parameter bodyParameter) => JsonConvert.SerializeObject(bodyParameter.Value);

        public T Deserialize<T>(RestResponse response) => JsonConvert.DeserializeObject<T>(response.Content);

        public ContentType ContentType { get; set; } = ContentType.Json;

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public ISerializer Serializer => this;
        public IDeserializer Deserializer => this;
        public string[] AcceptedContentTypes { get; } = ["application/json"];
        public SupportsContentType SupportsContentType { get; } = contentType => new List<string> { "application/json", "text/json", "text/x-json", "text/javascript", "*+json" }.Contains(contentType);
    }
}
