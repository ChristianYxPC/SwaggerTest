using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerTest.Configurations.Filters
{
    public class AutoRestSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                OpenApiArray enumValues = new OpenApiArray();
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    enumValues.Add(new OpenApiObject
                    {
                        ["value"] = new OpenApiInteger(Convert.ToInt32(enumValue)),
                        ["description"] = new OpenApiString(enumValue.ToString()),
                        ["name"] = new OpenApiString(enumValue.ToString())
                    });
                }
                schema.Extensions.Add(
                    "x-ms-enum",
                    new OpenApiObject
                    {
                        ["name"] = new OpenApiString(type.Name),
                        ["modelAsString"] = new OpenApiBoolean(true),
                        ["values"] = enumValues
                    }
                );
            };
        }
    }
}
