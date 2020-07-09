namespace Public.Api.Address
{
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.Legacy.Address.Requests;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using RestSharp;

    public partial class AddressController
    {
        [HttpPost("bosa/adressen")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddAddress(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaAddressRequest searchBody,
            CancellationToken cancellationToken = default)
            => await SearchBestAddAddressWithFormat(
                null,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                searchBody,
                cancellationToken);

        [HttpPost("bosa/adressen.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddAddressWithFormat(
            [FromRoute] string format,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaAddressRequest searchBody,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = ContentFormat.For(format, actionContextAccessor, Request);

            IRestRequest BackendRequest() => CreateBackendSearchBestAddRequest(searchBody);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendSearchBestAddRequest(BosaAddressRequest searchBody)
            => new RestRequest("adressen/bosa", Method.POST).AddJsonBodyOrEmpty(searchBody);
    }
}
