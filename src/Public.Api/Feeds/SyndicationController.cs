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

        [HttpGet("postal")]
        public IActionResult GetPostal(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/postinfo");

        [HttpGet("streetname")]
        public IActionResult GetStreetName(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/straatnamen");

        [HttpGet("address")]
        public IActionResult GetAddress(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/adressen");

        [HttpGet("parcel")]
        public IActionResult GetParcel(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/percelen");

        [HttpGet("building")]
        public IActionResult GetBuilding(CancellationToken cancellationToken) => new RedirectResult("/v1/feeds/gebouwen");
    }
}
