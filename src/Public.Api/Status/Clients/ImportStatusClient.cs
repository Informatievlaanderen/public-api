namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class ImportStatusClient : BaseStatusClient<IEnumerable<RegistryImportStatus>, ImportStatusList>
    {
        public ImportStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("crabimport/status");

        protected override IEnumerable<RegistryImportStatus> Map(ImportStatusList response)
            => response?
                .Select(MapToImportStatusResponse)
                .Where(status => status != null)
               ?? new List<RegistryImportStatus>();

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
