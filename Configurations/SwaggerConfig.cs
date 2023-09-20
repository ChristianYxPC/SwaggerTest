using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SwaggerTest.Configurations.Filters;
using SwaggerTest.Enums;
using SwaggerTest.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace SwaggerTest.Configurations
{
    public static class SwaggerConfig
    {
        static List<string> Versions = new List<string>()
        {
            "1",
            "1.0",
            "2"
        };

        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddControllers(option =>
            {
                option.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                //option.Conventions.Add(new ApiExplorerGetsOnlyConvention());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                //to enable swagger annotations like [SwaggerOperations]
                option.EnableAnnotations();

                option.SchemaFilter<AutoRestSchemaFilter>();
                option.SchemaFilter<DefaultSampleValueSchemaFilter>();

                option.OperationFilter<FromFormWithListOperationFilter>();

                option.MapType<Dictionary<GenderEnum, List<string>>>(() => new OpenApiSchema());
                option.SchemaFilter<DictionaryTKeyEnumTValueSchemaFilter>();

                option.OperationFilter<AuthResponsesOperationFilter>();
                foreach(var version in Versions)
                {
                    option.SwaggerDoc($"v{version}", new OpenApiInfo { Title = $"Swagger Test v{version}", Version = $"v{version}" });
                }
                //option.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Test v1", Version = "v1" });
                //option.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Swagger Test v1.0", Version = "v1.0" });
                //option.SwaggerDoc("v2", new OpenApiInfo { Title = "Swagger Test v2", Version = "v2" });
                option.ResolveConflictingActions(apidescription => apidescription.First());

                // OperationIds via Method name
                //option.CustomOperationIds(apiDesc =>
                //{
                //    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                //});

                // display api base on [ApiVersion({version})] and .SwaggerDoc("v{version}")
                // e.g [ApiVersion("1")] and .SwaggerDoc("v1")
                // e.g [ApiVersion("1.0")] and .SwaggerDoc("v1.0")
                option.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var controllerVersions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var methodVersions = methodInfo
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return methodVersions.Any()                                
                                ? methodVersions.Any(v => $"v{v}" == docName)
                                : controllerVersions.Any(v => $"v{v}" == docName) ||
                            (!controllerVersions.Any() && !methodVersions.Any() && docName == "v1");
                });

                // ignore obsolete controller methods
                option.IgnoreObsoleteActions();

                // ignore obsolute properties
                option.IgnoreObsoleteProperties();

                //Customize Operation Tags (e.g. for UI Grouping) or custom tags
                //option.TagActionsBy(api => api.HttpMethod);

                //Change Operation Sort Order (e.g. for UI Sorting)
                option.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

                // custom schema ids
                option.CustomSchemaIds((type) => type.FullName);

                //add Mappers
                option.TypeMappers();

                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "MyApi.xml");
                //option.IncludeXmlComments(filePath);
            });
        }

        public static void UseSwaggerConfig(this WebApplication? app)
        {
            app.UseSwagger(option =>
            {
                // serialize in v2 format
                //option.SerializeAsV2 = true;

                // change swagger route
                //option.RouteTemplate = "api-docs/{documentName}/swagger.json";

                // add bearer authentication
                //option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please enter a valid token",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});
                //option.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type=ReferenceType.SecurityScheme,
                //                Id="Bearer"
                //            }
                //        },
                //        new string[]{}
                //    }
                //});

                option.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> 
                    {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}", Description = "Https" },
                        new OpenApiServer { Url = $"http://localhost:5266", Description = "Http" }
                    };
                });
            });
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "swagger";
                //c.SwaggerEndpoint("v1/swagger.json", "Swagger Test");

                //OR
                //c.SwaggerEndpoint("swagger/v1/swagger.json", "Swagger Test");

                //OR
                // custom swagger route
                // change swagger route
                foreach (var version in Versions)
                {
                    c.SwaggerEndpoint($"v{version}/swagger.json", $"Swagger Test v{version}");
                }
                //c.SwaggerEndpoint("v1/swagger.json", "Swagger Test v1");
                //c.SwaggerEndpoint("v1.0/swagger.json", "Swagger Test v1.0");
                //c.SwaggerEndpoint("v2/swagger.json", "Swagger Test v2");

                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.ShowExtensions();
            });
        }


        /// <summary>
        /// add type mappers
        /// </summary>
        /// <param name="option"></param>
        private static void TypeMappers(this SwaggerGenOptions option)
        {
            option.MapType<PhoneNumber>(() => new OpenApiSchema { Type = "string" });
        }
    }
}
