using System;
using System.Text;

namespace Swashbuckle.AspNetCore.Annotations
{
    using System.Collections.Generic;

    /// <summary>
    /// Enriches Operation metadata for a given action method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerAuthorizeOperationAttribute : SwaggerOperationAttribute
    {
        private string[]? _authorize;

        public SwaggerAuthorizeOperationAttribute(string? summary = null, string? description = null) : base(summary, description)
        {
        }

        public string[]? Authorize
        {
            get => _authorize;
            set
            {
                _authorize = value;
                if (_authorize is null || _authorize.Length == 0)
                {
                    return;
                }

                var sb = new StringBuilder();
                if (_authorize.Length == 1)
                {
                    var scope = _authorize[0];
                    sb.AppendLine(scope.Contains(' ')
                        ? $"Authorization scopes: {scope}"
                        : $"Authorization scopes: `{scope}`");
                }
                else
                {
                    sb.AppendLine("Authorization scopes:");
                    foreach (var item in _authorize)
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine($"{System.Net.WebUtility.HtmlEncode(item)}");
                    }
                }
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine(System.Net.WebUtility.HtmlEncode(Description));
                Description = sb.ToString();
            }
        }
    }
}
