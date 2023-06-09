namespace Public.Api.Status.Clients.BackendResponse
{
    using System;
    using System.Collections.Generic;

    public class ConsumerStatusList : List<ConsumerStatus> { }

    public class ConsumerStatus
    {
        public string Name { get; set; }
        public DateTimeOffset LastProcessedMessage { get; set; }
    }
}
