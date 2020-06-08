namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using Api.Infrastructure;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class RejectInvalidQueryParametersFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Query.Keys.Any(x => x.Contains(".")))
                return;

            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            var format =
                context.GetValueFromHeader("format")
                ?? context.GetValueFromRouteData("format")
                ?? context.GetValueFromQueryString("format");

            var acceptType = request.GetTypedHeaders().DetermineAcceptType(format);
            var contentType = acceptType.ToMimeTypeString();

            request.Headers[HeaderNames.Accept] = contentType;

            throw new ApiException(
                $"Ongeldige parameters. Het gebruik van een prefix bij een parameter is niet geldig. Bekijk {configuration["DocsUrl"]} voor een overzicht van geldige parameters.",
                StatusCodes.Status400BadRequest);
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
