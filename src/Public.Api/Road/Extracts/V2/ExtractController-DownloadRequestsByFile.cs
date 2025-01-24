namespace Public.Api.Road.Extracts.V2
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure.Controllers.Attributes;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
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
            var sb = new StringBuilder();
            var context = HttpContext;
            sb.AppendLine("Headers:");
            foreach (var header in HttpContext.Request.Headers)
            {
                sb.AppendLine($"{header.Key}={header.Value}");
            }
            sb.AppendLine();

            if (context.Request.HasFormContentType)
            {
                sb.AppendLine("Form data:");
                foreach (var field in context.Request.Form)
                {
                    sb.AppendLine($"{field.Key}: {field.Value}");
                }
                sb.AppendLine();
            }

            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var bodyString = await reader.ReadToEndAsync();
                sb.AppendLine("Body:");
                sb.AppendLine(bodyString);
                context.Request.Body.Position = 0; // Reset the stream position
                sb.AppendLine();
            }

            logger.LogWarning(sb.ToString());


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
