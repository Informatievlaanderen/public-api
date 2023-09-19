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
        [HttpGet("wegen/activiteit/gebeurtenis/{id}/inhoud", Name = nameof(GetContent))]
        public async Task<IActionResult> GetContent(
            [FromRoute] long? id,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "changefeed/entry/{id}/content")
                    .AddParameter(nameof(id), id, ParameterType.UrlSegment);

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
