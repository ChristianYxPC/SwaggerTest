using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using SwaggerTest.Helpers;
using System.ComponentModel;

namespace SwaggerTest.Configurations.Filters
{
    public class FromFormWithListOperationFilter : IOperationFilter
    {
        ILogger<FromFormWithListOperationFilter> _logger;
        public FromFormWithListOperationFilter(ILogger<FromFormWithListOperationFilter> logger)
        {
            _logger = logger;
        }

        //[DebuggerDisplay(value: "Name", Name = nameof(Name), Target = typeof(string), TargetTypeName = "string", Type = "string")]
        [DebuggerDisplay(value: $"{nameof(Name)}: {{{nameof(Name)}}}, {nameof(IsRequired)}: {{{nameof(IsRequired)}}}", Name = nameof(Name), Target = typeof(string), TargetTypeName = "string", Type = "string")]
        private struct ApiParameterProperty
        {
            public string Name { get; }
            public OpenApiSchema Schema { get; }
            public bool IsRequired { get; }

            public ApiParameterProperty(string name, OpenApiSchema schema, bool isRequired)
            {
                Name = name;
                Schema = schema;
                IsRequired = isRequired;
            }

            public KeyValuePair<string, OpenApiSchema> GetNamedSchema()
            {
                return new KeyValuePair<string, OpenApiSchema>(Name, Schema);
            }
        }
        OperationFilterContext _context;
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            _context = context;

            var parameters = operation.Parameters;
            if (parameters == null)
                return;

            var @params = context.ApiDescription.ActionDescriptor.Parameters;
            if (parameters.Count == @params.Count)
                return;

            if (context.ApiDescription.RelativePath == "FromForm/List" /*|| context.ApiDescription.RelativePath == "FromForm/List/Item"*/)
            {
                if (@params.FirstOrDefault(p => p is ControllerParameterDescriptor paramDesc &&
                paramDesc.ParameterInfo.CustomAttributes.Any(pAtt => pAtt.AttributeType == typeof(FromFormAttribute))) is ControllerParameterDescriptor paramDesc)
                {
                    OpenApiSchema schema = GetParamApiSchema(paramDesc);
                    OpenApiMediaType paramFormData = new OpenApiMediaType()
                    {
                        Schema = schema,
                        Encoding = new Dictionary<string, OpenApiEncoding>(schema.Properties.Select(p => new KeyValuePair<string, OpenApiEncoding>(p.Key, new OpenApiEncoding() { Style = ParameterStyle.DeepObject })))
                    };
                    //operation.RequestBody.Content.Add("application/form-data", paramFormData);
                    operation.RequestBody.Content.Clear();
                    operation.RequestBody.Content.Add("multipart/form-data", paramFormData);
                }
            }
        }

        private OpenApiSchema GetParamApiSchema(ControllerParameterDescriptor paramDesc)
        {
            // if API Method Param has [FromForm]
            if (paramDesc.ParameterInfo.CustomAttributes != null && paramDesc.ParameterInfo.CustomAttributes.Any(pAtt => pAtt.AttributeType == typeof(FromFormAttribute)))
            {
                OpenApiSchema schema = new OpenApiSchema() { Type = "object" };
                List<ApiParameterProperty> apiParameterProperties = GetApiSchema(paramDesc.Name, paramDesc.ParameterType);
                schema.Properties = new Dictionary<string, OpenApiSchema>(apiParameterProperties.Select(scheme => scheme.GetNamedSchema()));
                schema.Required = new HashSet<string>(apiParameterProperties.Where(prop => prop.IsRequired).Select(prop => prop.Name));
                return schema;
            }
            return null;
        }

        /// <summary>
        /// return listrName[0].Prop1
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private List<ApiParameterProperty> GetApiSchema(string propertyName, Type propertyType)
        {
            List<ApiParameterProperty> newProps = new List<ApiParameterProperty>();
            var properties = propertyType.GetProperties();
            if (properties != null && properties.Any())
            {
                if (IsListType(propertyType))
                {
                    if (properties.FirstOrDefault(prop => prop.Name == "Item") is PropertyInfo itemInfo &&
                        itemInfo.PropertyType != null)
                    {
                        if (!propertyName.Contains('.'))
                        {
                            propertyName = "";
                        }
                        var listItemSchema = GetPropertyApiSchema(itemInfo.PropertyType);
                        if (listItemSchema == null)
                        {
                            foreach (var itemSchema in GetApiSchema($"{propertyName}[0]", itemInfo.PropertyType))
                            {
                                newProps.Add(itemSchema);
                            }
                        }
                        else
                        {
                            if (listItemSchema != null)
                            {
                                newProps.Add(new ApiParameterProperty($"{propertyName}[0]", listItemSchema, false));
                            }
                        }

                    }
                }
                else
                {
                    foreach (var prop in properties)
                    {
                        if (prop != null)
                        {
                            var newPropSchema = GetPropertyApiSchema(propertyName, prop);
                            if (newPropSchema != null)
                            {
                                newProps.Add(newPropSchema.Value);
                            }
                            else
                            {
                                foreach (var keyVal in GetApiSchema($"{propertyName}.{prop.Name}", prop.PropertyType))
                                {
                                    newProps.Add(keyVal);
                                }
                            }
                        }
                    }
                }
            }
            return newProps;
        }

        private ApiParameterProperty? GetPropertyApiSchema(string propertyName, PropertyInfo propertyInfo)
        {
            OpenApiSchema schema = GetPropertyApiSchema(propertyInfo.PropertyType);
            if (schema != null)
            {
                bool isRequired = propertyInfo.CustomAttributes.Any(propAtt => propAtt.AttributeType == typeof(RequiredAttribute));
                return new ApiParameterProperty($"{propertyName}.{propertyInfo.Name}", schema, isRequired);
            }

            return null;
        }

        private bool IsListType(Type propertyType)
        {
            return (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)) ||
                   propertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private OpenApiSchema GetPropertyApiSchema(Type propertyInfoType, DefaultValueAttribute? defaultAttribute = null)
        {
            string? format = null;
            string type = "";
            IOpenApiAny? exampleValue = null;
            switch (propertyInfoType.Name)
            {
                case nameof(System.Single): type = "single"; format = null; exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, 1)); break;
                case nameof(System.Byte): type = "string"; format = "binary"; exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, 1)); break;
                case nameof(System.Int16):
                case nameof(System.Int32):
                case nameof(System.Int64): /*type = "integer"; format = propertyInfoType.Name.ToLower(); break;*/
                case nameof(System.Double): type = "integer"; format = propertyInfoType.Name.ToLower(); exampleValue = new OpenApiInteger(GetDefaultValue(defaultAttribute, RandomHelper.GetRandomInt(1, 10))); break;
                case nameof(System.Boolean): type = "boolean"; format = null; exampleValue = new OpenApiBoolean(RandomHelper.GetRandomBoolean()); break;
                case nameof(System.String): type = "string"; format = null; exampleValue = new OpenApiString(GetDefaultValue(defaultAttribute, RandomHelper.GetRandomName())); break;
                case nameof(System.DateTime): type = "string"; format = "date-time"; exampleValue = new OpenApiString(DateTime.Now.ToLongDateString()); break;
                case nameof(IFormFile): type = "string"; format = "binary"; break;
                default:
                    if (propertyInfoType.IsEnum && _context.SchemaRepository.TryLookupByType(propertyInfoType, out OpenApiSchema referenceSchema))
                    {
                        return referenceSchema;
                    }
                    else if (Nullable.GetUnderlyingType(propertyInfoType) is Type valueType)
                    {
                        return GetPropertyApiSchema(valueType);
                    }
                    return null;
            }
            return new OpenApiSchema() { Type = type, Format = format, Example = exampleValue };
        }

        private T GetDefaultValue<T>(DefaultValueAttribute? defaultAttribute, T defaultValue)
        {
            return defaultAttribute?.Value is T tValue ? tValue : defaultValue;
        }

    }

}
