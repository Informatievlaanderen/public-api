namespace Public.Api.Status
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class ImportStatusResponse : List<RegistryImportStatus> {}

    public class RegistryImportStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public RegistryImportBatch LastCompletedImport { get; set; }

        [DataMember(Order = 3)]
        public RegistryImportBatch CurrentImport { get; set; }
    }

    public class RegistryImportBatch
    {
        [DataMember(Order = 1)]
        public DateTime From { get; set; }

        [DataMember(Order = 2)]
        public DateTime Until { get; set; }
    }
}
