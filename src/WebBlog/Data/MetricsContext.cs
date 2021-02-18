using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebBlog.Data
{
    public class MetricsContext : DbContext
    {
        private IOptions<ConfigOptions> Config { get; set; }

        public MetricsContext(DbContextOptions<MetricsContext> options) : base(options)
        {
        }

        public MetricsContext(IOptions<ConfigOptions> Configuration)
        {
            Config = Configuration;
        }

        public MetricsContext(DbContextOptions<MetricsContext> options, IOptions<ConfigOptions> Configuration) : base(options)
        {
            Config = Configuration;
        }

        public virtual DbSet<Metric> Metrics { get; set; }

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
        }
    }
}
