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
        private string _authorize = "";

        public SwaggerAuthorizeOperationAttribute(string? summary = null, string? description = null) : base(summary, description)
        {
        }

        public string Authorize
        {
            get => _authorize;
            set
            {
                _authorize = value;

                var sb = new StringBuilder();
                sb.AppendLine($"Authorization scopes: `{_authorize}`");
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine(Description);
                Description = sb.ToString();
            }
        }
    }

    /// <summary>
    /// Enriches Operation metadata for a given action method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerAuthorizesOperationAttribute : SwaggerOperationAttribute
    {
        private string[] _authorizationScopes = [];

        public SwaggerAuthorizesOperationAttribute(string? summary = null, string? description = null) : base(summary, description)
        {
        }

        public string[] AuthorizationScopes
        {
            get => _authorizationScopes;
            set
            {
                _authorizationScopes = value;
                if (_authorizationScopes.Length == 0)
                {
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Authorization scopes:");
                foreach (var item in _authorizationScopes)
                {
                    sb.AppendLine(item);
                }
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine(Description);
                Description = sb.ToString();
            }
        }
    }
}
