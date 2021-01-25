namespace Common.Infrastructure
{
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ContentFormat
    {
        public AcceptType ContentType { get; }
        public UrlExtension UrlExtension { get; }

        private ContentFormat(string urlFormat, AcceptType contentType)
        {
            UrlExtension = new UrlExtension(urlFormat);
            ContentType = contentType;
        }

        public static ContentFormat For(
            EndpointType endpointType,
            string urlFormat,
            ActionContext context)
        {
            var acceptType = DetermineAcceptType(context, urlFormat)
                .ValidateFor(endpointType);

            return new ContentFormat(urlFormat, acceptType);
        }

        public static ContentFormat For(
            EndpointType endpointType,
            ActionContext context)
        {
            var acceptType = DetermineAcceptType(context, string.Empty)
                .ValidateFor(endpointType);

            return new ContentFormat(string.Empty, acceptType);
        }

        public static AcceptType DetermineAcceptType(ActionContext context)
            => DetermineAcceptType(context, string.Empty);

        public static AcceptType DetermineAcceptType(ActionContext context, string urlFormat)
        {
            return context
                .HttpContext
                .Request
                .GetTypedHeaders()
                .DetermineAcceptType(
                    context.DetermineFormatParameter(urlFormat),
                    context.ActionDescriptor);
        }
    }
}
