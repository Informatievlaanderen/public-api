namespace Common.Infrastructure
{
    public class BackendResponse
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool CameFromCache { get; set; }

        public BackendResponse(string content, string contentType, bool cameFromCache)
        {
            Content = content;
            ContentType = contentType;
            CameFromCache = cameFromCache;
        }
    }
}
