namespace Public.Api.Road.Changes.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;

    public partial class ChangeFeedControllerV2
    {
        [HttpGet("wegen/activiteit/vorige", Name = nameof(GetPrevious))]
        public async Task<IActionResult> GetPrevious(
            [FromQuery] long? beforeEntry,
            [FromQuery] int? maxEntryCount,
            [FromQuery] string? filter,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "changefeed/previous")
                    .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
                    .AddParameter(nameof(beforeEntry), beforeEntry, ParameterType.QueryString)
                    .AddParameter(nameof(filter), filter, ParameterType.QueryString);

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
