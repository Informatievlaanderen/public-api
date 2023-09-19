namespace Public.Api.Road.Information
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class InformationController
    {
        [HttpGet("wegen/informatie", Name = nameof(GetInformation))]
        public async Task<IActionResult> GetInformation(CancellationToken cancellationToken)
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
