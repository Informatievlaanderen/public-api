namespace Public.Api.StreetName
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json.Converters;
    using RestSharp;
    using StreetNameRegistry.Api.Legacy.StreetName.Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public partial class StreetNameController
    {
        [HttpGet("bosa/straatnamen/{straatnaamId}")]
        public async Task<IActionResult> GetBestAddStreetName(
            [FromRoute] string straatnaamId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
            => await GetBestAddStreetNameWithFormat(
                null,
                straatnaamId,
                actionContextAccessor,
                ifNoneMatch,
                cancellationToken);

        [HttpGet("bosa/straatnamen/{straatnaamId}.{format}")]
        public async Task<IActionResult> GetBestAddStreetNameWithFormat(
            [FromRoute] string format,
            [FromRoute] string straatnaamId,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            // TODO: Implement something like GET, but for bosa

            return new BackendResponseResult(null);
        }
    }
}
