namespace Public.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    public class BackendResponseResultOptions
    {
        private IEnumerable<string> _forwardHeaders;

        public IEnumerable<string> ForwardHeaders
        {
            get => _forwardHeaders ?? Enumerable.Empty<string>();
            set => _forwardHeaders = value;
        }

        public static BackendResponseResultOptions ForBackOffice()
        {
            return new BackendResponseResultOptions { ForwardHeaders = new List<string> { "Location", "ETag" }};
        }

        public static BackendResponseResultOptions ForRead()
        {
            return new BackendResponseResultOptions { ForwardHeaders = new List<string> { "ETag", "X-Page-Complete" } };
        }
    }
}
