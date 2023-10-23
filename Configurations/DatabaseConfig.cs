using SwaggerTest.Services.Contexts;

namespace SwaggerTest.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfig(this IServiceCollection services)
        {
            services.AddDbContext<TestDbContext>();
        }
    }
}
