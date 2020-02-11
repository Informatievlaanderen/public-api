namespace Public.Api.Infrastructure
{
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class ApiProducesAttribute : ProducesAttribute
    {
        public ApiProducesAttribute()
            : base(
                "application/problem+json",
                //"application/problem+xml",
                AcceptTypes.Json//,
                //AcceptTypes.JsonLd,
                //AcceptTypes.Xml
                )
        {
        }
    }
}
