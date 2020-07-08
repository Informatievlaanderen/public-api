namespace Public.Api.Municipality
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using MunicipalityRegistry.Api.Legacy.Municipality.Requests;
    using RestSharp;

    public partial class MunicipalityController
    {
        [HttpPost("bosa/gemeenten")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddMunicipality(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaMunicipalityRequest searchBody,
            CancellationToken cancellationToken = default)
            => await SearchBestAddMunicipalityWithFormat(
                null,
                actionContextAccessor,
                responseOptions,
                ifNoneMatch,
                searchBody,
                cancellationToken);

        [HttpPost("bosa/gemeenten.{format}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddMunicipalityWithFormat(
            [FromRoute] string format,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaMunicipalityRequest searchBody,
            CancellationToken cancellationToken = default)
        {
            format = DetermineAndSetContentFormat(format, actionContextAccessor, Request);

            IRestRequest BackendRequest() => CreateBackendSearchBestAddRequest(searchBody);

            var value = await GetFromBackendAsync(
                format,
                BackendRequest,
                Request.GetTypedHeaders(),
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendSearchBestAddRequest(BosaMunicipalityRequest searchBody)
            => new RestRequest("gemeenten/bosa", Method.POST).AddJsonBodyOrEmpty(searchBody);
    }
}
