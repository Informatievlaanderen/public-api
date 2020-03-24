namespace Public.Api.Infrastructure
{
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class ApiProducesAttribute : ProducesAttribute
    {
        public ApiProducesAttribute()
            : base(
                AcceptTypes.JsonProblem,
                AcceptTypes.XmlProblem,
                AcceptTypes.Json,
                //AcceptTypes.JsonLd,
                AcceptTypes.Xml
                )
        {
        }
    }
}
