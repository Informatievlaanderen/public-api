namespace Public.Api.Road.Changes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.LastObservedPosition;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using ParcelRegistry.Api.Legacy.Parcel.Responses;
    using RestSharp;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

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
                cancellationToken);
            return new BackendResponseResult(response);
        }

        private static IRestRequest CreateBackendNextRequest(int? maxEntryCount, long? afterEntry) => new RestRequest("changefeed/next")
            .AddParameter(nameof(maxEntryCount), maxEntryCount, ParameterType.QueryString)
            .AddParameter(nameof(afterEntry), afterEntry, ParameterType.QueryString);
    }
}
