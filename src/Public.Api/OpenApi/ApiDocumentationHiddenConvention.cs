using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Public.Api.OpenApi
{
    public class ApiDocumentationHiddenConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            action.ApiExplorer.IsVisible = false;
        }
    }
}
