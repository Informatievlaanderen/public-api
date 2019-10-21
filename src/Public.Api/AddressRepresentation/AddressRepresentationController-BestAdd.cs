namespace Public.Api.AddressRepresentation
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.Address.Requests;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using RestSharp;

    public partial class AddressRepresentationController
    {
        [HttpPost("bosa/adresvoorstellingen")]
        public async Task<IActionResult> SearchBestAddAddressRepresentation(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaAddressRepresentationRequest searchBody,
            CancellationToken cancellationToken = default)
            => await SearchBestAddAddressRepresentationWithFormat(
                null,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                searchBody,
                cancellationToken);

        [HttpPost("bosa/adresvoorstellingen.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddAddressRepresentationWithFormat(
            [FromRoute] string format,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaAddressRepresentationRequest searchBody,
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

        private static IRestRequest CreateBackendSearchBestAddRequest(BosaAddressRepresentationRequest searchBody)
            => new RestRequest("adressen/bosa/adresvoorstellingen", Method.POST).AddJsonBodyOrEmpty(searchBody);
    }
}
