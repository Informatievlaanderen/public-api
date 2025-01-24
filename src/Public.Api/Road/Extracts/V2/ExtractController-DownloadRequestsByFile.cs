namespace Public.Api.Road.Extracts.V2
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Extracts;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extract/downloadaanvragen/perbestand")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult> PostDownloadRequestByFileV2(
            DownloadExtractByFileRequestBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] ILogger<ExtractControllerV2> logger,
            CancellationToken cancellationToken = default)
        {
            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response);

            RestRequest BackendRequest()
            {
                var request = CreateBackendRestRequest(Method.Post, "extracts/downloadrequests/byfile");

                request.AddParameter(nameof(body.Description), body.Description, ParameterType.GetOrPost);
                request.AddParameter(nameof(body.IsInformative), body.IsInformative, ParameterType.GetOrPost);
                if (body.Files is not null)
                {
                    foreach (var file in body.Files)
                    {
                        request.AddFile(nameof(body.Files), () =>
                        {
                            var copyStream = new MemoryStream();
                            file.OpenReadStream().CopyTo(copyStream);
                            copyStream.Position = 0;
                            return copyStream;
                        }, file.FileName);
                    }
                }

                request.AddHeader("Content-Type", "multipart/form-data");

                return request;
            }
        }
    }
}
