namespace Public.Api.Infrastructure
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BackendResponseResult : ContentResult
    {
        private readonly BackendResponse _response;

        public BackendResponseResult(BackendResponse response)
        {
            _response = response;
            Content = response.Content;
            ContentType = response.ContentType;
            StatusCode = StatusCodes.Status200OK;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("x-cached", _response.CameFromCache ? "yes" : "no");

            if (!string.IsNullOrWhiteSpace(_response.DownstreamVersion))
                context.HttpContext.Response.Headers.Add("x-basisregister-downstream-version", _response.DownstreamVersion);

            if (_response.CameFromCache)
                context.HttpContext.Response.Headers.Add("x-last-modified", _response.LastModified.ToString("O", CultureInfo.InvariantCulture));

            return base.ExecuteResultAsync(context);
        }
    }
}
