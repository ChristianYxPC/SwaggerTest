using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using SwaggerTest.Data;
using System.Security.AccessControl;

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

        #region AuditLogs
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EApplicationFileRequirement>().ToTable(nameof(EApplicationFileRequirements));
            modelBuilder.Entity<EApplicationMultipleFileRequirement>().ToTable(nameof(EApplicationMultipleFileRequirements));

            modelBuilder.Entity<HealthDecQuestionnaire>().ToTable(nameof(HealthDeclarationQuestionaires));
            modelBuilder.Entity<HealthDecQuestionnaireSubQuestion>().ToTable(nameof(HealthDeclarationQuestionaireSubQuestions));

            #region AuditLogModelConfig
            modelBuilder.Entity<AuditLog>()
                .Property(alP => alP.Timestamp)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<AuditLog>()
                .Property(alP => alP.KeyValues)
                .HasConversion(
                    keyValue => keyValue == null || !keyValue.Any() ? null : JsonConvert.SerializeObject(keyValue),
                    keyValueString => JsonConvert.DeserializeObject<Dictionary<string, object>>(keyValueString ?? "{}"));
            modelBuilder.Entity<AuditLog>()
                .Property(alP => alP.NewValues)
                .HasConversion(
                    newValues => newValues == null || !newValues.Any() ? null : JsonConvert.SerializeObject(newValues),
                    newValuesString => JsonConvert.DeserializeObject<Dictionary<string, object>>(newValuesString ?? "{}"));
            modelBuilder.Entity<AuditLog>()
                .Property(alP => alP.OldValues)
                .HasConversion(
                    oldValues => oldValues == null || !oldValues.Any() ? null : JsonConvert.SerializeObject(oldValues),
                    oldValuesString => JsonConvert.DeserializeObject<Dictionary<string, object>>(oldValuesString ?? "{}"));
            #endregion
            base.OnModelCreating(modelBuilder);
#if DEBUG
            //TestSeed(modelBuilder);
            SeedClass.Start(modelBuilder);
#endif
        }



        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDateChanges();
            List<AuditLog> newAuditLogs = AuditChanges();
            int saveChanges = await base.SaveChangesAsync(cancellationToken);
            SaveAudits(newAuditLogs);
            return saveChanges;
        }

        private void AddDateChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseData baseData)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseData.DateCreated = DateTime.Now;
                            break;
                        case EntityState.Deleted:
                            baseData.DateUpdated = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            baseData.DateUpdated = DateTime.Now;
                            break;
                    }
                }
            }
        }

        private List<AuditLog> AuditChanges()
        {
            //if (!this.AuditingAndEntityTimestampingEnabled)
            //{
            //    return null;
            //}

            ChangeTracker.DetectChanges();
            List<AuditLog> auditEntries = new List<AuditLog>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditLog()
                {
                    Entity = entry,
                    TableName = entry.Metadata.GetTableName() ?? string.Empty
                };

                if (entry.Entity is BaseData baseData)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.Action = AuditLogActionEnum.Insert.ToString();
                            auditEntry.ActionBy = baseData.CreatedBy;
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues = auditEntry.OldValues ?? new Dictionary<string, object>();
                            auditEntry.Action = AuditLogActionEnum.Delete.ToString();
                            auditEntry.ActionBy = baseData.UpdatedBy ?? "System";
                            break;
                        case EntityState.Modified:
                            auditEntry.Action = baseData.IsDeleted ? AuditLogActionEnum.Delete.ToString() : AuditLogActionEnum.Update.ToString();
                            auditEntry.ActionBy = baseData.UpdatedBy ?? "System";
                            break;
                    }
                }

                auditEntries.Add(auditEntry);

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified && property.OriginalValue?.ToString() != property.CurrentValue?.ToString())
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                AuditLogs.Add(auditEntry);
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
            //return auditEntries;
        }

        private void SaveAudits(List<AuditLog> auditLogs)
        {
            foreach (AuditLog auditLog in auditLogs)
            {
                if (auditLog.HasTemporaryProperties)
                {
                    foreach (PropertyEntry tempProp in auditLog.TemporaryProperties)
                    {
                        string propertyName = tempProp.Metadata.Name;
                        if (tempProp.Metadata.IsPrimaryKey())
                        {
                            auditLog.KeyValues[propertyName] = tempProp.CurrentValue;
                        }
                        if (auditLog.NewValues.ContainsKey(propertyName))
                        {
                            auditLog.NewValues[propertyName] = tempProp.CurrentValue;
                        }
                        if (auditLog.OldValues.ContainsKey(propertyName))
                        {
                            auditLog.OldValues[propertyName] = tempProp.CurrentValue;
                        }
                    }
                }
            }
            AuditLogs.AddRange(auditLogs);
            this.SaveChanges();
        } */
        #endregion
    }
}
