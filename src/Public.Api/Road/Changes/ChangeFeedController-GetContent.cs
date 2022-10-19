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
        [HttpGet("wegen/activiteit/gebeurtenis/{id}/inhoud", Name = nameof(GetContent))]
        public async Task<IActionResult> GetContent(
            [FromRoute] long? id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() => CreateBackendContentRequest(id);

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);
            return new BackendResponseResult(response);
        }

        private static RestRequest CreateBackendContentRequest(long? id) => new RestRequest("changefeed/entry/{id}/content")
                .AddParameter(nameof(id), id, ParameterType.UrlSegment);
    }
}
