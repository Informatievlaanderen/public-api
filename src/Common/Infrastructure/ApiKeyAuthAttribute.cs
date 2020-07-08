namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";

        private readonly string _validApiKeySet;

        public ApiKeyAuthAttribute(string validApiKeySet)
            => _validApiKeySet = validApiKeySet;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PublicApiController)
            {
                PublicApiController.DetermineAndSetFormat(
                    string.Empty,
                    context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>(),
                    context.HttpContext.Request);
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
                throw new ApiException("API key verplicht.", StatusCodes.Status401Unauthorized);

            var valiApiKeys = context
                .HttpContext
                .RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection($"ApiKeys:{_validApiKeySet}")
                .GetChildren()
                .Select(c => c.Value)
                .ToArray();

            if (!valiApiKeys.Contains(potentialApiKey.First()))
                throw new ApiException("Ongeldige API key.", StatusCodes.Status401Unauthorized);

            await next();
        }
    }
}
