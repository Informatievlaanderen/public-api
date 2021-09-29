namespace Public.Api.Road.Downloads
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    public partial class DownloadController
    {
        [HttpGet("wegen/download/voor-editor")]
        public async Task GetForEditor(CancellationToken cancellationToken = default)
        {
            using (var response = await _httpClient.GetAsync("download/for-editor", HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                Response.Headers.Add(HeaderNames.ContentType, response.Content.Headers.ContentType.MediaType);
                Response.Headers.Add(HeaderNames.ContentDisposition, response.Content.Headers.ContentDisposition.ToString());

                await responseStream.CopyToAsync(Response.Body, cancellationToken);
            }
        }
    }
}
