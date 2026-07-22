namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using Feeds.V2;
    using Feeds.V3.Change;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.OpenApi;
    using Swashbuckle.AspNetCore.SwaggerGen;
    public class ProblemDetailsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Responses is null || operation.Responses.Count == 0)
            {
                return;
            }
            foreach (var operationResponse in operation.Responses)
            {
                var content = operationResponse.Value.Content;
                if (content is null || content.Count == 0)
                {
                    continue;
                }
                if (operationResponse.Key.StartsWith("2"))
                {
                    content.Remove("application/problem+json");
                    content.Remove("application/problem+xml");
                }
                if (operationResponse.Key.StartsWith("4") || operationResponse.Key.StartsWith("5"))
                {
                    content.Remove("application/json");
                    content.Remove("application/ld+json");
                    content.Remove("application/xml");
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
            {
                operation.Parameters = new List<IOpenApiParameter>();
            }
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor &&
                (descriptor.ControllerTypeInfo.Name.Equals(nameof(FeedV2Controller))
                 || descriptor.ControllerTypeInfo.Name.Equals(nameof(ChangeFeedV3Controller))))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Description = "x-api-key header met verkregen API key.",
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                    Required = true
                });
            }
            else
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Description = "x-api-key header met verkregen API key (optioneel).",
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                    Required = false // set to false if this is optional
                });
            }
        }
    }
}
