namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class BackOfficeStatusClient : BaseStatusClient<RegistryBackOfficeStatusResponse, List<BackOfficeStatus>>
    {
        public BackOfficeStatusClient(string registry, RestClient restClient)
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
                            Name = "BackOffice",
                            CurrentPosition = status.Position,
                            MaxPosition = status.MaxPosition
                        })
            };
    }
}
