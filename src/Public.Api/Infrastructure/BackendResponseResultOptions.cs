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
    }
}
