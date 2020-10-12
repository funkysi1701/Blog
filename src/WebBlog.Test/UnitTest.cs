using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
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
        public async Task BlogServiceTest()
        {
            var blogs = await BlogService.GetBlogsAsync();
            Assert.NotEmpty(blogs);
        }

    }
}
