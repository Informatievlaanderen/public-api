namespace Public.Api.Infrastructure
{
    using System.Net.Mime;
    using Common.Infrastructure;
    using RestSharp;

    public static class AddJsonBodyOrEmptyExtension
    {
        public static RestRequest AddJsonBodyOrEmpty(this RestRequest request, object body)
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
