namespace Public.Api.Status.Clients
{
    using BackendResponse;
    using Responses;
    using RestSharp;

    public sealed class SnapshotStatusClient : BaseStatusClient<RegistrySnapshotStatusResponse, SnapshotStatus>
    {
        public SnapshotStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("snapshots");

        protected override RegistrySnapshotStatusResponse Map(SnapshotStatus response)
            => new RegistrySnapshotStatusResponse()
            {
                Name = "Snapshot",
                FailedSnapshotsCount = response.FailedSnapshotsCount,
                DifferenceInDaysOfLastVerification = response.DifferenceInDaysOfLastVerification
            };
    }
}
