using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebBlog.Data
{
    public class MetricsContext : DbContext
    {
        public DbSet<Metric> Metrics { get; set; }
        private IConfiguration Config { get; set; }

        public MetricsContext(IConfiguration Configuration)
        {
            Config = Configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseCosmos(
                    Config.GetValue<string>("CosmosDBURI"),
                    Config.GetValue<string>("CosmosDBKey"),
                    databaseName: "Metrics");

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