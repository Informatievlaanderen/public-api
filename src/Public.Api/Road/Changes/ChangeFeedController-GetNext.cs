namespace Public.Api.Road.Changes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ChangeFeedController
    {
        [HttpGet("wegen/activiteit/volgende", Name = nameof(GetNext))]
        public async Task<IActionResult> GetNext(
            [FromQuery] long? afterEntry,
            [FromQuery] int? maxEntryCount,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendNextRequest(maxEntryCount, afterEntry);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);
            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendNextRequest(int? maxEntryCount, long? afterEntry) => new RestRequest("changefeed/next")
            .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
            .AddParameter(nameof(afterEntry), afterEntry, ParameterType.QueryString);
    }
}
