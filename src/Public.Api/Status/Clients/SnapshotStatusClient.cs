namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public sealed class SnapshotStatusClient : BaseStatusClient<RegistrySnapshotStatusResponse, List<SnapshotStatus>>
    {
        public SnapshotStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("snapshots");

        protected override RegistrySnapshotStatusResponse Map(List<SnapshotStatus> response)
            => new RegistrySnapshotStatusResponse()
            {
                projections = response
                    .Select(status =>
                        new RegistrySnapshotStatus()
                        {
                            Name = "Snapshot",
                            FailedSnapshotsCount = status.FailedSnapshotsCount,
                            LastVerifiedSnapshotTimestamp = status.LastSnapshotVerificationTimestamp
                        })
            };
    }
}
