namespace Public.Api.Status
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Clients;
    using Common.Infrastructure.Controllers;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;
    using AcceptTypes = Be.Vlaanderen.Basisregisters.Api.AcceptTypes;
    using Version = Infrastructure.Version.Version;

   // [ApiVersion(Version.Current)]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("status")]
    [ApiExplorerSettings(GroupName = "Status", IgnoreApi = true)]
    [ApiOrder(ApiOrder.Status)]
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
        public async Task<IActionResult> GetImportStatus(
            [FromServices] IEnumerable<ImportStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var importStatuses = ImportStatusResponse.From(keyValuePairs);

            return Ok(importStatuses);
        }

        /// <summary>
        /// Vraag de status van projections op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de projections gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("projection")]
        [ProducesResponseType(typeof(ProjectionStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetProjectionStatus(
            [FromServices] IEnumerable<ProjectionStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var projectionStatuses = ProjectionStatusResponse.From(keyValuePairs);

            return Ok(projectionStatuses);
        }

        /// <summary>
        /// Vraag de status van caches op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de caches gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("cache")]
        [ProducesResponseType(typeof(CacheStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetCacheStatus(
            [FromServices] IEnumerable<CacheStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var cacheStatuses = CacheStatusResponse.From(keyValuePairs);

            return Ok(cacheStatuses);
        }

        /// <summary>
        /// Vraag de status van syndication op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de syndication gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("syndication")]
        [ProducesResponseType(typeof(SyndicationStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetSyndicationStatus(
            [FromServices] IEnumerable<SyndicationStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var syndicationStatuses = SyndicationStatusResponse.From(keyValuePairs);

            return Ok(syndicationStatuses);
        }

        /// <summary>
        /// Vraag de status van de producers op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de producers gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("producer")]
        [ProducesResponseType(typeof(ProjectionStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetProducerStatus(
            [FromServices] IEnumerable<ProducerStatusClient> producerClients,
            [FromServices] IEnumerable<ProducerSnapshotOsloStatusClient> snapshotClients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairsProducer =
                (await producerClients.GetStatuses(cancellationToken)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var keyValuePairsSnapshot = await snapshotClients.GetStatuses(cancellationToken);

            foreach (var (key, value) in keyValuePairsSnapshot)
            {
                try
                {
                    if (keyValuePairsProducer.ContainsKey(key) &&
                        keyValuePairsProducer[key] is null && value is null)
                    {
                        continue;
                    }

                    if (keyValuePairsProducer.ContainsKey(key))
                    {
                        if (keyValuePairsProducer[key] is null && value is not null)
                        {
                            keyValuePairsProducer[key] = value;
                        }
                        else
                        {
                            keyValuePairsProducer[key].Projections =
                                keyValuePairsProducer[key].Projections.Concat(value.Projections);
                        }
                    }
                    else
                    {
                        keyValuePairsProducer.Add(key, value);
                    }
                }
                catch (Exception)
                {
                    if (keyValuePairsProducer.ContainsKey(key))
                    {
                        keyValuePairsProducer[key] = null;
                    }
                    else
                    {
                        keyValuePairsProducer.Add(key, null);
                    }
                }
            }

            var projectionStatuses = ProjectionStatusResponse.From(keyValuePairsProducer);

            return Ok(projectionStatuses);
        }

        /// <summary>
        /// Vraag de status van consumer op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de consumer gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("consumer")]
        [ProducesResponseType(typeof(ConsumerStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetConsumerStatus(
            [FromServices] IEnumerable<ConsumerStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var consumerStatusResponses = ConsumerStatusResponse.From(keyValuePairs);

            return Ok(consumerStatusResponses);
        }

        /// <summary>
        /// Vraag de status van parcel importer grb op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de importer grb gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("importergrb")]
        [ProducesResponseType(typeof(ImportStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetImporterGrbStatus(
            [FromServices] IEnumerable<ImporterGrbStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var importStatuses = ImportStatusResponse.From(keyValuePairs);

            return Ok(importStatuses);
        }

        /// <summary>
        /// Vraag de status van de backoffice projecties op.
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als opvragen van de status van de backoffice projections gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("backoffice")]
        [ProducesResponseType(typeof(SyndicationStatusResponse), StatusCodes.Status200OK)]
        [HttpCacheExpiration(MaxAge = DefaultStatusCaching)]
        public async Task<IActionResult> GetBackOfficeStatus(
            [FromServices] IEnumerable<BackOfficeStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var keyValuePairs = await clients.GetStatuses(cancellationToken);
            var backOfficeStatusResponses = BackOfficeStatusResponse.From(keyValuePairs);

            return Ok(backOfficeStatusResponses);
        }
    }
}
