namespace Common.Infrastructure
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Primitives;

    public class StreamingBackendResponse : IBackendResponse
    {
        public string? ResponseContentType { get; }
        public string? ContentDisposition { get; }
        public Stream ResponseStream { get; }
        public IEnumerable<KeyValuePair<string, StringValues>> ResponseHeaders { get; }

        public StreamingBackendResponse(
            string? responseContentType,
            string? contentDisposition,
            Stream responseStream,
            IEnumerable<KeyValuePair<string, StringValues>> responseHeaders)
        {
            ResponseContentType = responseContentType;
            ContentDisposition = contentDisposition;
            ResponseStream = responseStream;
            ResponseHeaders = responseHeaders;
        }
    }
}
