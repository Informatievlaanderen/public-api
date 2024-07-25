namespace Public.Api.Status.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class SnapshotStatusResponse : ListResponse<RegistrySnapshotStatusResponse> { }

    public class RegistrySnapshotStatusResponse
    {
        [DataMember(Order = 1)]
        public IEnumerable<RegistrySnapshotStatus> projections { get; set; }
    }

    public class RegistrySnapshotStatus
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public int FailedSnapshotsCount { get; set; }

        [DataMember(Order = 3)]
        public DateTimeOffset LastVerifiedSnapshotTimestamp { get; set; }
    }
}
