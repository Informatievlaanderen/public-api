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
                    sb.AppendLine($"Authorization scopes: `{_authorize[0]}`");
                }
                else
                {
                    sb.AppendLine("Authorization scopes:");
                    foreach (var item in _authorize)
                    {
                        sb.AppendLine(item);
                    }
                }
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine(Description);
                Description = sb.ToString();
            }
        }
    }
}
