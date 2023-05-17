using System;
using System.Text;

namespace Swashbuckle.AspNetCore.Annotations
{
    /// <summary>
    /// Enriches Operation metadata for a given action method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerAuthorizeOperationAttribute : SwaggerOperationAttribute
    {
        private string _authorize = "";

        public SwaggerAuthorizeOperationAttribute(string summary = null, string description = null) : base(summary, description)
        {
            Authorize = "";
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
}
