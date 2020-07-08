namespace Common.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;

    public enum EndpointType
    {
        Legacy,
        Sync
    }

    public class ApiProducesAttribute : ProducesAttribute
    {
        public ApiProducesAttribute()
            : this(EndpointType.Legacy) { }

        public ApiProducesAttribute(EndpointType endpointType) : base(AcceptTypes.Json)
        {
            var contentTypes = new MediaTypeCollection();

            switch (endpointType)
            {
                case EndpointType.Legacy:
                    contentTypes.Add(AcceptTypes.JsonProblem);
                    contentTypes.Add(AcceptTypes.XmlProblem);
                    contentTypes.Add(AcceptTypes.Json);
                    //contentTypes.Add(AcceptTypes.JsonLd);
                    contentTypes.Add(AcceptTypes.Xml);
                    break;

                case EndpointType.Sync:
                    contentTypes.Add(AcceptTypes.XmlProblem);
                    contentTypes.Add(AcceptTypes.Atom);
                    contentTypes.Add(AcceptTypes.Xml);
                    break;

                default:
                    contentTypes.Add(AcceptTypes.Json);
                    break;
            }

            ContentTypes = contentTypes;
        }
    }
}
