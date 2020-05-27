namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class RejectInvalidQueryParametersFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Request.Query.Keys.Any(x => x.Contains(".")))
                throw new ApiException(
                    "Ongeldige parameters. Het gebruik van een prefix bij een parameter is niet geldig.",
                    StatusCodes.Status400BadRequest);
        }
    }
}
