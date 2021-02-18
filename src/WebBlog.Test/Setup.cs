using Microsoft.EntityFrameworkCore;
using System;
using WebBlog.Data;

namespace WebBlog.Test
{
    public class Setup
    {
        private readonly MetricsContext context;
        public readonly DbContextOptions<MetricsContext> Options;

        public Setup()
        {
            var builder = new DbContextOptionsBuilder<MetricsContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            Options = builder.Options;
            context = new MetricsContext(Options);
        }
    }
}
