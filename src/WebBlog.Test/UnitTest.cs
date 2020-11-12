using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WebBlog.Data;
using Xunit;

namespace WebBlog.Test
{
    public class UnitTest
    {
        private BlogService BlogService;

        public UnitTest()
        {
            var config = Config.GetIConfiguration(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var mockFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient
            {
                BaseAddress = new Uri(config.GetValue<string>("DEVTOURL"))
            };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            BlogService = new BlogService(mockFactory.Object, config);
        }

        [Fact]
        public void Test()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task GetBlogsAsync()
        {
            var blogs = await BlogService.GetBlogsAsync();
            Assert.NotEmpty(blogs);
        }

        [Fact]
        public async Task GetBlogPostAsync()
        {
            var blogs = await BlogService.GetBlogsAsync();
            var post = await BlogService.GetBlogPostAsync(blogs[0].Id);
            Assert.NotEmpty(blogs);
        }

        [Fact]
        public void AppVersionInfoTest()
        {
            var mockEnvironment = new Mock<IHostEnvironment>();
            mockEnvironment.Setup(m => m.ContentRootPath).Returns("");
            var t = new AppVersionInfo(mockEnvironment.Object);
            Assert.Equal("123456", t.BuildId);
            Assert.Equal(DateTime.UtcNow.ToString("yyyyMMdd") + ".0", t.BuildNumber);
            Assert.Equal("LOCALBUILD", t.GitHash);
            Assert.Equal("LBUILD", t.ShortGitHash);
        }
    }
}