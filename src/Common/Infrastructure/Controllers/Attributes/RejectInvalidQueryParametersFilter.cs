namespace Common.Infrastructure.Controllers.Attributes
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class RejectInvalidQueryParametersFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.Request.Query.Keys.Any(x => x.Contains(".")))
                return;

            context.SetContentFormatAcceptType();

            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            throw new ApiException(
                $"Ongeldige parameters. Het gebruik van een prefix bij een parameter is niet geldig. Bekijk {configuration["DocsUrl"]} voor een overzicht van geldige parameters.",
                StatusCodes.Status400BadRequest);
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
