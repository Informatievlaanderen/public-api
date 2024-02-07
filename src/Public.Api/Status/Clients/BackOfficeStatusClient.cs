namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Common.Infrastructure;
    using Responses;
    using RestSharp;

    public class BackOfficeStatusClient : BaseStatusClient<RegistryBackOfficeStatusResponse, List<BackOfficeStatus>>
    {
        public BackOfficeStatusClient(string registry, TraceRestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("backoffice");

        protected override RegistryBackOfficeStatusResponse Map(List<BackOfficeStatus> response)
            => new RegistryBackOfficeStatusResponse()
            {
                projections = response
                    .Select(status =>
                        new RegistryBackOfficeStatus()
                        {
                            Name = status.ProjectionName,
                            CurrentPosition = status.Position,
                            MaxPosition = status.MaxPosition
                        })
            };
    }
}
