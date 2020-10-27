namespace Public.Api.Status.Clients.BackendResponse
{
    using System;
    using System.Collections.Generic;

    public class ImportStatusList : List<ImportStatus> { }

    public class ImportStatus
    {
        public string Name { get; set; }
        public ImportStatusBatchScope LastCompletedBatch { get; set; }
        public ImportStatusBatchScope CurrentBatch { get; set; }
    }

    public class ImportStatusBatchScope
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset Until { get; set; }
    }
}
