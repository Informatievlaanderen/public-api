namespace Public.Api.Infrastructure.Swagger
{
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
                    operationResponse.Value.Content.Remove("application/xml");
                }
            }
        }
    }
}
