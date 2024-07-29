namespace Public.Api.Status.Clients.BackendResponse
{
    public class SnapshotStatus
    {
        public int FailedSnapshotsCount { get; set; }
        public int DifferenceInDaysOfLastVerification { get; set; }
    }
}
