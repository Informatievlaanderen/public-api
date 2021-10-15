namespace Common.Infrastructure.Controllers.Attributes
{
    using Extensions;
    using Microsoft.AspNetCore.Mvc;

    public class ApiConsumesAttribute : ConsumesAttribute
    {
        public ApiConsumesAttribute()
            : this(EndpointType.BackOffice) { }

        public ApiConsumesAttribute(EndpointType endpointType) : base(AcceptTypes.Json)
        {
            ContentTypes = endpointType.Consumes();
        }
    }
}
