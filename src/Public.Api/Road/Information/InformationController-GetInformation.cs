namespace Public.Api.Road.Information
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class InformationController
    {
        [HttpGet("wegen/informatie", Name = nameof(GetInformation))]
        public async Task<IActionResult> GetInformation(CancellationToken cancellationToken)
        {
            RestRequest BackendRequest() =>
                new RestRequest("information");

            var response = await GetFromBackendAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);
            return new BackendResponseResult(response);
        }
    }
}
