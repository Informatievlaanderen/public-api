namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Infrastructure.Controllers.Attributes;
    using Feeds;
    using Feeds.V2;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class ProblemDetailsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var operationResponse in operation.Responses)
            {
                if (operationResponse.Key.StartsWith("2"))
                {
                    operationResponse.Value.Content.Remove("application/problem+json");
                    operationResponse.Value.Content.Remove("application/problem+xml");
                }

                if (operationResponse.Key.StartsWith("4") || operationResponse.Key.StartsWith("5"))
                {
                    operationResponse.Value.Content.Remove("application/json");
                    operationResponse.Value.Content.Remove("application/ld+json");
                    operationResponse.Value.Content.Remove("application/xml");
                }
            }
        }
    }

    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class XApiFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            {
                return;
            }

            var apiKeyAuthAttribute =
                descriptor.MethodInfo.GetCustomAttributes(typeof(ApiKeyAuthAttribute), true).FirstOrDefault() as ApiKeyAuthAttribute
                ?? descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(ApiKeyAuthAttribute), true).FirstOrDefault() as ApiKeyAuthAttribute;
            if (apiKeyAuthAttribute is not null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Description = apiKeyAuthAttribute.IsOptional
                        ? "x-api-key header met verkregen API key (optioneel)."
                        : "x-api-key header met verkregen API key.",
                    Schema = new OpenApiSchema { Type = "string" },
                    Required = !apiKeyAuthAttribute.IsOptional
                });
            }
        }
    }
}
