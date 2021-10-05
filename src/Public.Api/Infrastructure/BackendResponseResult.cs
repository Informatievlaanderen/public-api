namespace Public.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;

    public class BackendResponseResult : ContentResult
    {
        private readonly BackendResponse _response;
        private readonly BackendResponseResultOptions _options;

        public BackendResponseResult(
            BackendResponse response,
            BackendResponseResultOptions options = default)
        {
            _response = response;
            _options = options ?? new BackendResponseResultOptions();
            Content = response.Content;
            ContentType = response.ContentType;
            StatusCode = (int)response.StatusCode;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("x-cached", _response.CameFromCache ? "yes" : "no");

            if (!string.IsNullOrWhiteSpace(_response.DownstreamVersion))
                context.HttpContext.Response.Headers.Add("x-basisregister-downstream-version", _response.DownstreamVersion);

            if (_response.CameFromCache)
                context.HttpContext.Response.Headers.Add("x-last-modified", _response.LastModified.ToString("O", CultureInfo.InvariantCulture));

            foreach (var headerToForward in _options.ForwardHeaders)
            {
                var headerFromResponse = _response.ResponseHeaders
                    .SingleOrDefault(responseHeader => responseHeader.Key == headerToForward);

                if (!headerFromResponse.Equals(new KeyValuePair<string, StringValues>()))
                {
                    context.HttpContext.Response.Headers.Add(headerFromResponse.Key, headerFromResponse.Value);
                }
            }

            return base.ExecuteResultAsync(context);
        }
    }
}
