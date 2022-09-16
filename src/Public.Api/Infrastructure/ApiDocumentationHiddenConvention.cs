namespace Public.Api.Infrastructure
{
    using System;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    public class ApiDocumentationHiddenConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var notGetTicket = !action.ActionMethod.Name.Equals("GetTicket", StringComparison.InvariantCultureIgnoreCase);
            var endsWithTicket = action.ActionMethod.Name.EndsWith("Ticket", StringComparison.InvariantCultureIgnoreCase)
                || action.ActionMethod.Name.EndsWith("Tickets", StringComparison.InvariantCultureIgnoreCase);

            if (notGetTicket && endsWithTicket)
            {
                action.ApiExplorer.IsVisible = false;
            }
        }
    }
}
