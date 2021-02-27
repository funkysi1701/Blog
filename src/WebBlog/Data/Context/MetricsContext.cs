using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebBlog.Data.Context
{
    public class MetricsContext : DbContext
    {
        private IOptions<CosmosOptions> Config { get; set; }

        public MetricsContext(DbContextOptions<MetricsContext> options) : base(options)
        {
        }

        public MetricsContext(IOptions<CosmosOptions> Configuration)
        {
            Config = Configuration;
        }

        public MetricsContext(DbContextOptions<MetricsContext> options, IOptions<CosmosOptions> Configuration) : base(options)
        {
            Config = Configuration;
        }

        public virtual DbSet<Metric> Metrics { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseCosmos(
                    Config.Value.CosmosDBURI,
                    Config.Value.CosmosDBKey,
                    databaseName: Config.Value.CosmosName);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Store");

            modelBuilder.Entity<Metric>()
                .ToContainer("Metrics");

            modelBuilder.Entity<Metric>()
                .HasNoDiscriminator();

            modelBuilder.Entity<Metric>()
                .HasPartitionKey(o => o.PartitionKey);

            modelBuilder.Entity<Metric>()
                .UseETagConcurrency();

            modelBuilder.Entity<Profile>()
                .ToContainer("Metrics");

            modelBuilder.Entity<Profile>()
                .HasNoDiscriminator();

            modelBuilder.Entity<Profile>()
                .HasPartitionKey(o => o.PartitionKey);

            modelBuilder.Entity<Profile>()
                .UseETagConcurrency();
        }
    }
}
