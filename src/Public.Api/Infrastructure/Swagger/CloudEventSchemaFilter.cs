namespace Public.Api.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using CloudNative.CloudEvents;
    using Microsoft.OpenApi;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public sealed class CloudEventSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(CloudEvent))
                return;

            if (schema is not OpenApiSchema cloudEventSchema)
                return;

            cloudEventSchema.Properties = new Dictionary<string, IOpenApiSchema>();
            cloudEventSchema.Properties.Add("specversion", new OpenApiSchema { Type = JsonSchemaType.String, Example = "1.0" });
            cloudEventSchema.Properties.Add("id", new OpenApiSchema { Type = JsonSchemaType.String });
            cloudEventSchema.Properties.Add("type", new OpenApiSchema { Type = JsonSchemaType.String });
            cloudEventSchema.Properties.Add("source", new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" });
            cloudEventSchema.Properties.Add("time", new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" });
            cloudEventSchema.Properties.Add("datacontenttype", new OpenApiSchema { Type = JsonSchemaType.String });
            cloudEventSchema.Properties.Add("dataschema", new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" });
            cloudEventSchema.Properties.Add("data", new OpenApiSchema { Type = JsonSchemaType.Object });
            // Add extension attributes
            cloudEventSchema.Properties.Add("basisregisterseventtype", new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Description = "Basisregister-specifieke event type." });
            cloudEventSchema.Properties.Add("basisregisterscausationid", new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Description = "Identifier om wijzigingen met elkaar te correleren o.b.v. het veroorzakend proces." });
            cloudEventSchema.AdditionalPropertiesAllowed = true;
        }
    }
}
