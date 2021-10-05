namespace Public.Api.Road.Changes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class ChangeFeedController
    {
        [HttpGet("wegen/activiteit/begin", Name = nameof(GetHead))]
        public async Task<IActionResult> GetHead(
            [FromQuery] int? maxEntryCount,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            IRestRequest BackendRequest() => CreateBackendHeadRequest(maxEntryCount);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken);
            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendHeadRequest(int? maxEntryCount) => new RestRequest("changefeed/head")
            .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString);
    }
}
