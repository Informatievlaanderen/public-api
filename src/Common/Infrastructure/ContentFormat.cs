namespace Common.Infrastructure
{
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            string urlFormat,
            IActionContextAccessor actionContextAccessor,
            HttpRequest request)
        {
            var actionContext = actionContextAccessor.ActionContext;
            var format = actionContext.DetermineFormatParameter(urlFormat);

            var acceptType = request
                .GetTypedHeaders()
                .DetermineAcceptType(format, actionContext.ActionDescriptor);

            return new ContentFormat(urlFormat, acceptType);
        }
    }
}
