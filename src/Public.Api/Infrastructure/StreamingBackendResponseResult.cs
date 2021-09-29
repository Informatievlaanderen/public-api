namespace Public.Api.Infrastructure
{
    using System.IO;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class StreamingBackendResponseResult : ActionResult
    {
        private readonly Stream _stream;
        private readonly string _contentType;
        private readonly string _contentDisposition;

        public StreamingBackendResponseResult(StreamingBackendResponse streamingBackendResponse)
        {
            _stream = streamingBackendResponse.ResponseStream;
            _contentType = streamingBackendResponse.ResponseContentType;
            _contentDisposition = streamingBackendResponse.ContentDisposition;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            response.Headers.Add(HeaderNames.ContentType, _contentType);
            response.Headers.Add(HeaderNames.ContentDisposition, _contentDisposition);

            await _stream.CopyToAsync(response.Body, context.HttpContext.RequestAborted);
        }
    }
}
