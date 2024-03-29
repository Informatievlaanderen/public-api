namespace Public.Api.Road.Information.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;

    public partial class InformationControllerV2
    {
        [HttpGet("wegen/informatie", Name = nameof(GetInformationV2))]
        public async Task<IActionResult> GetInformationV2(CancellationToken cancellationToken)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "information");

            var response = await GetFromBackendAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);
            return new BackendResponseResult(response);
        }
    }
}
