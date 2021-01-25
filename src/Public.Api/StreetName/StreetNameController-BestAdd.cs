namespace Public.Api.StreetName
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Infrastructure;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using RestSharp;
    using StreetNameRegistry.Api.Legacy.StreetName.Requests;

    public partial class StreetNameController
    {
        [HttpPost("bosa/straatnamen", Name = nameof(SearchBestAddStreetNameWithFormat))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SearchBestAddStreetNameWithFormat(
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] IOptions<MunicipalityOptions> responseOptions,
            [FromHeader(Name = HeaderNames.IfNoneMatch)] string ifNoneMatch,
            [FromBody] BosaStreetNameRequest searchBody,
            CancellationToken cancellationToken = default)
        {
            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            IRestRequest BackendRequest() => CreateBackendSearchBestAddRequest(searchBody);

            var value = await GetFromBackendAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);

            return BackendListResponseResult.Create(value, Request.Query, responseOptions.Value.VolgendeUrl);
        }

        private static IRestRequest CreateBackendSearchBestAddRequest(BosaStreetNameRequest searchBody)
            => new RestRequest("straatnamen/bosa", Method.POST).AddJsonBodyOrEmpty(searchBody);
    }
}
