namespace Common.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private const string ApiKeyQueryName = "apikey";

        private readonly string _validApiKeySet;

        public ApiKeyAuthAttribute(string validApiKeySet)
            => _validApiKeySet = validApiKeySet;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            void RefuseAccess(HttpContext httpContext, string message)
            {
                if (!(context.Controller is PublicApiController))
                    return;

                var actionContext = httpContext
                    .RequestServices
                    .GetRequiredService<IActionContextAccessor>()
                    .ActionContext;

                var acceptType = ContentFormat.DetermineAcceptType(httpContext.Request, actionContext);

                httpContext.Request.Headers[HeaderNames.Accept] = acceptType.ToProblemResponseMimeTypeString();

                throw new ApiException(message, StatusCodes.Status401Unauthorized);
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialHeaderApiKey) &&
                !context.HttpContext.Request.Query.TryGetValue(ApiKeyQueryName, out var potentialQueryApiKey))
                RefuseAccess(context.HttpContext, "API key verplicht.");

            var potentialApiKey = string.Empty;

            if (!string.IsNullOrWhiteSpace(potentialQueryApiKey))
                potentialApiKey = potentialQueryApiKey;

            if (!string.IsNullOrWhiteSpace(potentialHeaderApiKey))
                potentialApiKey = potentialHeaderApiKey;

            if (string.IsNullOrWhiteSpace(potentialApiKey))
                RefuseAccess(context.HttpContext, "API key verplicht.");

            var valiApiKeys = context
                .HttpContext
                .RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection($"ApiKeys:{_validApiKeySet}")
                .GetChildren()
                .Select(c => c.Value)
                .ToArray();

            if (!valiApiKeys.Contains(potentialApiKey))
                RefuseAccess(context.HttpContext, "Ongeldige API key.");

            await next();
        }
    }
}
