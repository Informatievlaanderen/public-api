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
        [HttpGet("wegen/activiteit/volgende", Name = nameof(GetNextV2))]
        public async Task<IActionResult> GetNextV2(
            [FromQuery] long? afterEntry,
            [FromQuery] int? maxEntryCount,
            [FromQuery] string? filter,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "changefeed/next")
                    .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
                    .AddParameter(nameof(afterEntry), afterEntry, ParameterType.QueryString)
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
