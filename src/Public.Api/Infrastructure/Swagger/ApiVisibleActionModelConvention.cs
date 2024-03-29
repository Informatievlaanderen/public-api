namespace Public.Api.Infrastructure.Swagger;

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class ApiVisibleActionModelConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        var actionIgnored = action.ActionMethod.GetCustomAttribute<ApiExplorerSettingsAttribute>()?.IgnoreApi == true;
        if (actionIgnored)
        {
            return;
        }

        var isVisible = action.Controller.ControllerType
            .GetCustomAttributes<ApiVisibleAttribute>(true)
            .Select(x => x.Visible)
            .FirstOrDefault();
            
        action.ApiExplorer.IsVisible = isVisible;
    }
}
