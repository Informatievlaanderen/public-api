namespace Public.Api.Infrastructure.Swagger
{
    using CloudNative.CloudEvents;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public sealed class CloudEventSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(CloudEvent))
                return;

            schema.Properties.Clear();
            schema.Properties.Add("specversion", new OpenApiSchema { Type = "string", Example = new OpenApiString("1.0") });
            schema.Properties.Add("id", new OpenApiSchema { Type = "string" });
            schema.Properties.Add("type", new OpenApiSchema { Type = "string" });
            schema.Properties.Add("source", new OpenApiSchema { Type = "string", Format = "uri" });
            schema.Properties.Add("time", new OpenApiSchema { Type = "string", Format = "date-time" });
            schema.Properties.Add("datacontenttype", new OpenApiSchema { Type = "string" });
            schema.Properties.Add("dataschema", new OpenApiSchema { Type = "string", Format = "uri" });
            schema.Properties.Add("data", new OpenApiSchema { Type = "object" });
            // Add extension attributes
            schema.Properties.Add("basisregisterseventtype", new OpenApiSchema { Type = "string" });
            schema.Properties.Add("basisregisterscausationid", new OpenApiSchema { Type = "string" });
            schema.AdditionalPropertiesAllowed = true;
        }
    }
}
