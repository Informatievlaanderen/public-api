namespace Public.Api.StreetName
{
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using RestSharp;
    using StreetNameRegistry.Api.Legacy.StreetName.Requests;

    public partial class StreetNameController
    {
        [HttpPost("bosa/straatnamen")]
        public async Task<IActionResult> SearchBestAddStreetName(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaStreetNameRequest searchBody,
            CancellationToken cancellationToken = default)
            => await SearchBestAddStreetNameWithFormat(
                null,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                searchBody,
                cancellationToken);

        [HttpPost("bosa/straatnamen.{format}")]
        public async Task<IActionResult> SearchBestAddStreetNameWithFormat(
            [FromRoute] string format,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaStreetNameRequest searchBody,
            CancellationToken cancellationToken = default)
        {
            format = !string.IsNullOrWhiteSpace(format)
                ? format
                : actionContextAccessor.ActionContext.GetValueFromHeader("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                  ?? actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            IRestRequest BackendRequest() => CreateBackendSearchBestAddRequest(searchBody);

            var value = await GetFromBackendAsync(
                format,
                BackendRequest,
                Request.GetTypedHeaders(),
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendSearchBestAddRequest(BosaStreetNameRequest searchBody)
            => new RestRequest("straatnamen/bosa", Method.POST).AddJsonBodyOrEmpty(searchBody);
    }
}
