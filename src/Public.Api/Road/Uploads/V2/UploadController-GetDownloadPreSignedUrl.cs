namespace Public.Api.Road.Uploads.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    public partial class UploadControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpGet("wegen/upload/{identifier}/presignedurl")]
        public async Task<ActionResult> GetDownloadPreSignedUrlForUpload(
            [FromRoute] string identifier,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            CancellationToken cancellationToken = default)
        {
            var value = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());

            RestRequest BackendRequest() =>
                CreateBackendRestRequest(Method.Get, "upload/{identifier}/presignedurl")
                    .AddParameter("identifier", identifier, ParameterType.UrlSegment);
        }
    }
}
