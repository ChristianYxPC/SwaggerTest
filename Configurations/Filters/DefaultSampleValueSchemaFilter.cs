using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SwaggerTest.Helpers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace SwaggerTest.Configurations.Filters
{
    public class DefaultSampleValueSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (PropertyInfo propertyInfo in context.Type.GetProperties())
            {

                // Look for class attributes that have been decorated with "[DefaultAttribute(...)]".
                DefaultValueAttribute defaultAttribute = propertyInfo
                    .GetCustomAttribute<DefaultValueAttribute>();

                //if (defaultAttribute != null)
                //{
                //    foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
                //    {
                //        // Only assign default value to the proper element.
                //        if (ToCamelCase(propertyInfo.Name) == property.Key)
                //        {
                //            switch(defaultAttribute.Value.GetType().Name)
                //            {
                //                case nameof(String):
                //                    property.Value.Example = new OpenApiString((string)defaultAttribute.Value) ;
                //                    break;
                //            }
                //            property.Value.Example = GetPropertySampleValue(defaultAttribute.Value.GetType());
                //            break;
                //        }
                //    }
                //}


                foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
                {
                    if (ToCamelCase(propertyInfo.Name) == property.Key)
                    {
                        if (defaultAttribute != null)
                        {
                        }
                        property.Value.Example = GetPropertySampleValue(propertyInfo.PropertyType);
                        break;
                    }
                }
            }
        }

        private IOpenApiAny? GetPropertySampleValue(Type propertyInfoType, DefaultValueAttribute? defaultAttribute = null)
        {
            IOpenApiAny? exampleValue = null;
            switch (propertyInfoType.Name)
            {
                case nameof(System.Single): exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, 1)); break;
                case nameof(System.Byte): exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, 1)); break;
                case nameof(System.Int16):
                case nameof(System.Int32):
                case nameof(System.Int64): /*type = "integer"; format = propertyInfoType.Name.ToLower(); break;*/
                case nameof(System.Double): exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, RandomHelper.GetRandomInt(1, 10))); break;
                case nameof(System.Boolean): exampleValue = new OpenApiBoolean(RandomHelper.GetRandomBoolean()); break;
                case nameof(System.String): exampleValue = new OpenApiString(GetDefaultValue(defaultAttribute, RandomHelper.GetRandomName())); break;
                case nameof(System.DateTime): exampleValue = new OpenApiString(DateTime.Now.ToLongDateString()); break;
                case nameof(IFormFile): new OpenApiString("file"); break;
                default:
                    //if (propertyInfoType.IsEnum && _context.SchemaRepository.TryLookupByType(propertyInfoType, out OpenApiSchema referenceSchema))
                    //{
                    //    return referenceSchema;
                    //}
                    //else if (Nullable.GetUnderlyingType(propertyInfoType) is Type valueType)
                    //{
                    //    return GetPropertyApiSchema(valueType);
                    //}
                    return null;
            }
            return exampleValue;
        }

        private T GetDefaultValue<T>(DefaultValueAttribute? defaultAttribute, T defaultValue)
        {
            return defaultAttribute?.Value is T tValue ? tValue : defaultValue;
        }


        private string ToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}