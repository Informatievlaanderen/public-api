using RestSharp;

namespace Common.Infrastructure.Extensions
{
    public static class IfMatchHeaderExtensions
    {
        public static RestRequest AddHeaderIfMatch(this RestRequest request, string headerName, string? ifMatch)
        {
            if (ifMatch is not null)
            {
                request.AddHeader(HeaderNames.IfMatch, ifMatch);
            }

            return request;
        }
    }
}
