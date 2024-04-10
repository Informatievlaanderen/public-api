namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using BackendResponse;
    using Responses;
    using RestSharp;

    public class SyndicationStatusClient : BaseStatusClient<RegistrySyndicationStatusResponse, List<SyndicationStatus>>
    {
        public SyndicationStatusClient(string registry, RestClient restClient)
            : base(registry, restClient) { }

        protected override RestRequest CreateStatusRequest()
            => new RestRequest("syndication");

        protected override RegistrySyndicationStatusResponse Map(List<SyndicationStatus> response)
            => new RegistrySyndicationStatusResponse
            {
                Syndications = response
                    .Select(status =>
                        new RegistrySyndicationStatus
                        {
                            Name = status.ProjectionName,
                            CurrentPosition = status.Position
                        })
            };
    }
}
