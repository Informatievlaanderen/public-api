namespace Public.Api.Status.Clients.BackendResponse
{
    using System;

    public class SnapshotStatus
    {
        public int FailedSnapshotsCount { get; set; }
        public DateTimeOffset LastSnapshotVerificationTimestamp { get; set; }
    }
}
