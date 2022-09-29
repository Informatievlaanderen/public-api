namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    public class ApiDocumentationHiddenConvention : IActionModelConvention
    {
        private readonly IEnumerable<string> _hiddenMethods;

        public ApiDocumentationHiddenConvention(IEnumerable<string> hiddenMethods)
        {
            _hiddenMethods = hiddenMethods;
        }

        public void Apply(ActionModel action)
        {
            if (_hiddenMethods.Contains(action.ActionMethod.Name))
            {
                action.ApiExplorer.IsVisible = false;
            }
        }
    }
}
