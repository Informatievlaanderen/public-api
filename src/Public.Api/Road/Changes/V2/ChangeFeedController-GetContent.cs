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
