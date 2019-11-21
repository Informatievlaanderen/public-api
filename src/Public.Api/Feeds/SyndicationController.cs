namespace Public.Api.Feeds
{
    using System.Threading;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersionNeutral]
    [Route("syndication/feed/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SyndicationController : ControllerBase
    {
        [HttpGet("municipality")]
        public IActionResult GetMunicipality(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/gemeenten");

        [HttpGet("municipality.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetMunicipality(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/gemeenten.{format}");

        [HttpGet("postal")]
        public IActionResult GetPostal(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/postinfo");

        [HttpGet("postal.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetPostal(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/postinfo.{format}");

        [HttpGet("streetname")]
        public IActionResult GetStreetName(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/straatnamen");

        [HttpGet("streetname.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetStreetName(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/straatnamen.{format}");

        [HttpGet("address")]
        public IActionResult GetAddress(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/adressen");

        [HttpGet("address.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetAddress(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/adressen.{format}");

        [HttpGet("parcel")]
        public IActionResult GetParcel(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/percelen");

        [HttpGet("parcel.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetParcel(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/percelen.{format}");

        [HttpGet("building")]
        public IActionResult GetBuilding(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/gebouwen");

        [HttpGet("building.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetBuilding(string format, CancellationToken cancellationToken) => new RedirectResult($"/v1/feeds/gebouwen.{format}");
    }
}
