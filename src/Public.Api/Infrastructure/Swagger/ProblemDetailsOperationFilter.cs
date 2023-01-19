namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
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
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor &&
                (descriptor.ControllerTypeInfo.Name.Equals(nameof(FeedController)) || descriptor.ControllerTypeInfo.Name.Equals(nameof(FeedV2Controller))))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Description = "x-api-key header met verkregen API key.",
                    Schema = new OpenApiSchema { Type = "string" },
                    Required = true
                });
            }
            else
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Description = "x-api-key header met verkregen API key (optioneel).",
                    Schema = new OpenApiSchema { Type = "string" },
                    Required = false // set to false if this is optional
                });
        }
    }
}
