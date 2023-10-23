using Microsoft.EntityFrameworkCore;
using SwaggerTest.Data;

namespace SwaggerTest.Services.Contexts
{
    public class TestDbContext : DbContext
    {

        //dbsets
        #region Tables
        public DbSet<DataTypeTable> DataTypeTables { get; set; }

        #endregion

        ConfigurationManager _configuration;
        public TestDbContext(DbContextOptions<TestDbContext> options, ConfigurationManager configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("TESTDB"),
                sqlOption => sqlOption.EnableRetryOnFailure(3));
#if DEBUG
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
#endif
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
