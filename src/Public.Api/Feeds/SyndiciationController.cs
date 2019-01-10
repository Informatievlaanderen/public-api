namespace Public.Api.Feeds
{
    using System.Threading;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersionNeutral]
    [Route("syndication/feed/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SyndiciationController : ControllerBase
    {
        [HttpGet("municipality")]
        public IActionResult GetMunicipality(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/gemeenten");

        [HttpGet("municipality.{format}")]
        public IActionResult GetMunicipality(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/gemeenten.{format}");

        [HttpGet("postal")]
        public IActionResult GetPostal(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/postinfo");

        [HttpGet("postal.{format}")]
        public IActionResult GetPostal(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/postinfo.{format}");

        [HttpGet("streetname")]
        public IActionResult GetStreetName(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/straatnamen");

        [HttpGet("streetname.{format}")]
        public IActionResult GetStreetName(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/straatnamen.{format}");

        [HttpGet("address")]
        public IActionResult GetAddress(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/adressen");

        [HttpGet("address.{format}")]
        public IActionResult GetAddress(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/adressen.{format}");

        [HttpGet("parcel")]
        public IActionResult GetParcel(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/percelen");

        [HttpGet("parcel.{format}")]
        public IActionResult GetParcel(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/percelen.{format}");
    }
}
