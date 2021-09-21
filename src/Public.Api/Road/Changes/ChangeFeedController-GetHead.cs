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
        [HttpGet("wegen/activiteit", Name = nameof(GetHead))]
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
