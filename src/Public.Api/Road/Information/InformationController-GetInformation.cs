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
            IRestRequest BackendRequest() => CreateBackendInformationRequest();

            var response = await GetFromBackendAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                cancellationToken);
            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendInformationRequest() => new RestRequest("information", Method.GET);
    }
}
