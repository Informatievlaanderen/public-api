namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class ImportStatusResponse : ListResponse<IEnumerable<RegistryImportStatus>>
    {
        public ImportStatusResponse()
        { }
        
        private ImportStatusResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }

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
