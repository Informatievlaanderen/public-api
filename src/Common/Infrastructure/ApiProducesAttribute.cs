namespace Common.Infrastructure
{
    using Extensions;
    using Microsoft.AspNetCore.Mvc;

    public class ApiProducesAttribute : ProducesAttribute
    {
        public ApiProducesAttribute()
            : this(EndpointType.Legacy) { }

        public ApiProducesAttribute(EndpointType endpointType) : base(AcceptTypes.Json)
        {
            ContentTypes = endpointType.Produces();
        }
    }
}
