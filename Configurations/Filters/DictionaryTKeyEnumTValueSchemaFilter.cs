using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerTest.Configurations.Filters
{
    public class DictionaryTKeyEnumTValueSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Only run for fields that are a Dictionary<Enum, TValue>
            if (!context.Type.IsGenericType || !context.Type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)))
            {
                return;
            }

            var keyType = context.Type.GetGenericArguments()[0];
            var valueType = context.Type.GetGenericArguments()[1];

            if (!keyType.IsEnum)
            {
                return;
            }

            schema.Type = "object";
            schema.Properties = keyType.GetEnumNames().ToDictionary(name => name, name => context.SchemaGenerator.GenerateSchema(valueType,  context.SchemaRepository));
        }
    }
}
