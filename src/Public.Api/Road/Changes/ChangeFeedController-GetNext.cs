namespace Public.Api.Road.Changes
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class ChangeFeedController
    {
        [HttpGet("wegen/activiteit/volgende", Name = nameof(GetNext))]
        public async Task<IActionResult> GetNext(
            [FromQuery] long? afterEntry,
            [FromQuery] int? maxEntryCount,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "changefeed/next")
                    .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
                    .AddParameter(nameof(afterEntry), afterEntry, ParameterType.QueryString);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);
            return new BackendResponseResult(response);
        }
    }
}
