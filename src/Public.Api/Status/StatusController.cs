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
    using Common.Infrastructure.Controllers;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("status")]
    [ApiExplorerSettings(GroupName = "Status", IgnoreApi = true)]
    [ApiOrder(Order = ApiOrder.Status)]
    [Produces(AcceptTypes.Json)]
    public class StatusController : PublicApiController
    {
        /// <summary>
        /// Vraag de status van importers op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de importers gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("import")]
        [ProducesResponseType(typeof(ImportStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> Get(
            [FromServices] IEnumerable<ImportRestClient> clients,
            CancellationToken cancellationToken = default)
        {
            var importStatuses = (await GetImportStatuses(clients, cancellationToken))
                .Aggregate(
                    new ImportStatusResponse(),
                    (response, repositoryStatuses) =>
                    {
                        response.AddRange(repositoryStatuses);
                        return response;
                    });

            importStatuses.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));

            return Ok(importStatuses);
        }

        private async Task<IEnumerable<IEnumerable<RegistryImportStatus>>> GetImportStatuses(
            IEnumerable<ImportRestClient> clients,
            CancellationToken cancellationToken)
        {
            return await Task.WhenAll(
                clients
                    .AsParallel()
                    .Select(async client => await GetRegistryImportStatuses(client, cancellationToken))
            );
        }

        private async Task<IEnumerable<RegistryImportStatus>> GetRegistryImportStatuses(
            IRestClient client,
            CancellationToken cancellationToken)
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
