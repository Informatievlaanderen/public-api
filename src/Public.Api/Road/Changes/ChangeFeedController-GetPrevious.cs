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
        [HttpGet("wegen/activiteit/vorige", Name = nameof(GetPrevious))]
        public async Task<IActionResult> GetPrevious(
            [FromQuery] long? beforeEntry,
            [FromQuery] int? maxEntryCount,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendPreviousRequest(maxEntryCount, beforeEntry);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);
            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendPreviousRequest(int? maxEntryCount, long? beforeEntry) => new RestRequest("changefeed/previous")
            .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
            .AddParameter(nameof(beforeEntry), beforeEntry, ParameterType.QueryString);
    }
}
