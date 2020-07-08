namespace Common.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;

    public class ContentFormat
    {
        public AcceptType ContentType { get; }
        public string UrlExtension { get; }

        private ContentFormat(string urlFormat, AcceptType contentType)
        {
            UrlExtension = urlFormat.IsNullOrWhiteSpace() ? "" : $".{urlFormat}";
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
