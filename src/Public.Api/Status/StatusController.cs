namespace Public.Api.Status
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Api.Messages;
    using Common.Infrastructure;
    using Common.Infrastructure.Modules;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("status")]
    [ApiExplorerSettings(GroupName = "Status", IgnoreApi = true)]
    [ApiOrder(Order = ApiOrder.Status)]
    [Produces(AcceptTypes.Json)]
    public class StatusController : ApiController
    {
        /// <summary>
        /// Vraag de status van importers op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de importers gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [ProducesResponseType(typeof(ImportStatusResponse), StatusCodes.Status200OK)]
        [HttpGet("import")]
        public async Task<IActionResult> Get(
            [FromServices] IEnumerable<ImportRestClient> clients,
            CancellationToken cancellationToken = default)
        {
            var importStatuses = new ImportStatusResponse();
            foreach (var client in clients)
            {
                var responses = await GetImportStatuses(client, cancellationToken);
                importStatuses.AddRange(responses);
            }

            importStatuses.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));

            return Ok(importStatuses);
        }

        private async Task<IEnumerable<RegistryImportStatus>> GetImportStatuses(IRestClient client, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteAsync<IEnumerable<ImportStatus>>(new RestRequest("crabimport/status"), cancellationToken);

            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK && response.Data != null)
                return response.Data.Select(MapToImportStatusResponse);

            return new List<RegistryImportStatus>();
        }

        private static RegistryImportStatus MapToImportStatusResponse(ImportStatus status)
        {
            RegistryImportBatch MapImportBatch(ImportStatusBatchScope batch)
            {
                if (batch == null)
                    return null;

                return new RegistryImportBatch
                {
                    From = batch.From.UtcDateTime,
                    Until = batch.Until.UtcDateTime
                };
            }

            return new RegistryImportStatus
            {
                Name = status.Name,
                LastCompletedImport = MapImportBatch(status.LastCompletedBatch),
                CurrentImport = MapImportBatch(status.CurrentBatch)
            };
        }
    }
}
