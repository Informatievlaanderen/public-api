namespace Public.Api.Status
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Clients;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;

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
            [FromServices] IEnumerable<ImportStatusClient> clients,
            CancellationToken cancellationToken = default)
        {
            var importStatuses = (await clients.GetStatuses(cancellationToken))
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
    }
}
