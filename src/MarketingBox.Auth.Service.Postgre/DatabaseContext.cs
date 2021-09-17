using MarketingBox.Auth.Service.Postgre.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Auth.Service.Postgre
{
    public class DatabaseContext : DbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "auth-service";

        private const string UserTableName = "users";

        public DbSet<UserEntity> Users { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public static ILoggerFactory LoggerFactory { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetUserEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable(UserTableName);
            modelBuilder.Entity<UserEntity>()
                .HasKey(e => new {e.TenantId, Email = e.ExternalUserId });
            modelBuilder.Entity<UserEntity>()
                .HasIndex(e => new { e.TenantId, e.Username })
                .IsUnique();
            modelBuilder.Entity<UserEntity>()
                .HasIndex(e => new { e.TenantId, e.EmailEncrypted })
                .IsUnique();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
