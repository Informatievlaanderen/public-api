namespace Common.Infrastructure
{
    using System.IO;

    public class StreamingBackendResponse : IBackendResponse
    {
        public string? ResponseContentType { get; }
        public string? ContentDisposition { get; }
        public Stream ResponseStream { get; }

        public StreamingBackendResponse(string? responseContentType, string? contentDisposition, Stream responseStream)
        {
            ResponseContentType = responseContentType;
            ContentDisposition = contentDisposition;
            ResponseStream = responseStream;
        }
    }
}
