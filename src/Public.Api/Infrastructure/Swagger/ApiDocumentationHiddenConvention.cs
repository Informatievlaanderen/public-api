namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    public class ApiDocumentationHiddenConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var hiddenMethods = new List<string>
            {
                "CompleteTicket",
                "CreateTicket",
                "DeleteTicket",
                "GetTickets",
                "PendingTicket"
            };

            if (hiddenMethods.Contains(action.ActionMethod.Name))
            {
                action.ApiExplorer.IsVisible = false;
            }
        }
    }
}
