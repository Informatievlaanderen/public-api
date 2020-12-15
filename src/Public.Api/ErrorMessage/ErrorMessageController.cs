namespace Public.Api.ErrorMessage
{
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Foutmeldingen", IgnoreApi = true)]
    [ApiOrder(Order = ApiOrder.Status)]
    [Produces(AcceptTypes.Json, AcceptTypes.Xml)]
    public class ErrorMessageController : PublicApiController
    {
        /// <summary>Vraag foutmelding details op.</summary>
        /// <param name="configuration"></param>
        /// <param name="errorId"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de foutmelding gelukt is.</response>
        /// <response code="302">Als foutmelding details als html wordt opgevraagd.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("foutmeldingen/{errorId}")]
        [ProducesResponseType(typeof(ErrorMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public IActionResult GetAllEventsMarkdown(
            [FromServices] IConfiguration configuration,
            [FromRoute] string errorId,
            CancellationToken cancellationToken = default)
        {
            if (Request.IsHtmlRequest())
                 return new RedirectResult(string.Format(configuration["ErrorMessageUrl"], errorId));

            // todo: lookup error message details for ID
            return NotFound($"Foutmelding {errorId} werd niet gevonden");
        }
    }
}
