namespace Public.Api.Road.Extracten
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Public.Api.Infrastructure;
    using RestSharp;
    using RoadRegistry.BackOffice.Api.Extracten;

    public partial class ExtractControllerV2
    {
        [ApiKeyAuth("Road", AllowAuthorizationHeader = true)]
        [HttpPost("wegen/extracten/downloadaanvragen/perbestand")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult> PostDownloadRequestByFile(
            ExtractDownloadaanvraagPerBestandBody body,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] RoadExtractDownloadRequestsByFileToggle toggle,
            CancellationToken cancellationToken = default)
        {
            if (!toggle.FeatureEnabled)
            {
                return NotFound();
            }

            var response = await GetFromBackendWithBadRequestAsync(
                AcceptType.Json,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(response, BackendResponseResultOptions.ForBackOffice());

            RestRequest BackendRequest()
            {
                var request = CreateBackendRestRequest(Method.Post, "extracten/downloadaanvragen/perbestand");

                request.AddParameter(nameof(body.Beschrijving), body.Beschrijving, ParameterType.GetOrPost);
                request.AddParameter(nameof(body.Informatief), body.Informatief, ParameterType.GetOrPost);
                if (body.Bestanden is not null)
                {
                    foreach (var bestand in body.Bestanden)
                    {
                        request.AddFile(nameof(body.Bestanden), () =>
                        {
                            var copyStream = new MemoryStream();
                            bestand.OpenReadStream().CopyTo(copyStream);
                            copyStream.Position = 0;
                            return copyStream;
                        }, bestand.FileName);
                    }
                }

                request.AddHeader("Content-Type", "multipart/form-data");

                return request;
            }
        }
    }
}
