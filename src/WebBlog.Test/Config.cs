using Microsoft.Extensions.Configuration;

namespace WebBlog.Test
{
    public static class Config
    {
        public static IConfiguration GetIConfiguration(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
