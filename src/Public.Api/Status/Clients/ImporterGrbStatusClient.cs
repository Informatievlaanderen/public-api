namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class ImporterGrbStatusClient : BaseStatusClient<IEnumerable<RegistryImportStatus>, ImportStatus>
    {
        public ImporterGrbStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("importergrb");

        protected override IEnumerable<RegistryImportStatus> Map(ImportStatus response)
            => new List<RegistryImportStatus>{MapToImportStatusResponse(response)};

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
