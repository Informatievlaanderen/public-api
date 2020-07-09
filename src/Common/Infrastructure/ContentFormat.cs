namespace Common.Infrastructure
{
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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
            var acceptType = DetermineAcceptType(request, actionContextAccessor.ActionContext, urlFormat);

            return new ContentFormat(urlFormat, acceptType);
        }

        public static AcceptType DetermineAcceptType(HttpRequest request, ActionContext context)
            => DetermineAcceptType(request, context, string.Empty);

        public static AcceptType DetermineAcceptType(HttpRequest request, ActionContext context, string urlFormat)
        {
            return request
                .GetTypedHeaders()
                .DetermineAcceptType(
                    context.DetermineFormatParameter(urlFormat),
                    context.ActionDescriptor);
        }
    }
}
