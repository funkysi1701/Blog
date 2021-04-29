using Microsoft.EntityFrameworkCore;
using System;
using WebBlog.Data.Context;

namespace WebBlog.Test
{
    public class Setup
    {
        public readonly DbContextOptions<MetricsContext> Options;

        public Setup()
        {
            var builder = new DbContextOptionsBuilder<MetricsContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            Options = builder.Options;
        }
    }
}
