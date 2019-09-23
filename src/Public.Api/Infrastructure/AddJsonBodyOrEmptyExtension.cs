namespace Public.Api.Infrastructure
{
    using System.Net.Mime;
    using Microsoft.Net.Http.Headers;
    using RestSharp;

    public static class AddJsonBodyOrEmptyExtension
    {
        public static IRestRequest AddJsonBodyOrEmpty(this RestRequest request, object body)
        {
            if (body == null)
            {
                request.AddHeader(HeaderNames.ContentType, MediaTypeNames.Application.Json);
                return request;
            }

            return request.AddJsonBody(body);
        }
    }
}
