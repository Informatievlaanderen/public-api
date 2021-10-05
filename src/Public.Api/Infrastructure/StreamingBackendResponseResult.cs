namespace Public.Api.Infrastructure
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class StreamingBackendResponseResult : ActionResult
    {
        private readonly StreamingBackendResponse _response;
        private readonly BackendResponseResultOptions _options;

        public StreamingBackendResponseResult(
            StreamingBackendResponse streamingBackendResponse,
            BackendResponseResultOptions options = default)
        {
            _response = streamingBackendResponse;
            _options = options ?? new BackendResponseResultOptions();
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            response.Headers.Add(HeaderNames.ContentType, _response.ResponseContentType);
            response.Headers.Add(HeaderNames.ContentDisposition, _response.ContentDisposition);

            foreach (var headerToForward in _options.ForwardHeaders)
            {
                var headerFromResponse = _response.ResponseHeaders
                    .SingleOrDefault(responseHeader => responseHeader.Key == headerToForward);

                if (!headerFromResponse.Equals(default))
                {
                    context.HttpContext.Response.Headers.Add(headerFromResponse.Key, headerFromResponse.Value);
                }
            }

            await _response.ResponseStream.CopyToAsync(response.Body, context.HttpContext.RequestAborted);
        }
    }
}
