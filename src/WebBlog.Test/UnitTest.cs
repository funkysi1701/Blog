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
        public void BlogPostsTest()
        {
            var bp = new BlogPosts
            {
                Body_Html = "html",
                Canonical_Url = "url",
                Comments_Count = 0,
                Cover_Image = "",
                Description = "",
                Id = 1,
                Page_Views_Count = 0,
                Path = "",
                Positive_Reactions_Count = 0,
                Published =true,
                Published_At = DateTime.UtcNow,
                Published_Timestamp = DateTime.UtcNow,
                Slug = "",
                Title = "",
                Type_of = "",
                Url = "url",
                User = new User
                {
                    Username = "simon",
                    Name ="Simon",
                    Twitter_Username ="funkysi1701"
                }
            };
            Assert.Equal("html", bp.Body_Html);
            Assert.Equal("url", bp.Canonical_Url);
            Assert.Equal(0, bp.Comments_Count);
            Assert.Equal("", bp.Cover_Image);
            Assert.Equal("", bp.Description);
            Assert.Equal(1, bp.Id);
            Assert.Equal(0, bp.Page_Views_Count);
            Assert.True(bp.Published);
            Assert.Equal(DateTime.UtcNow, bp.Published_At);
            Assert.Equal(DateTime.UtcNow, bp.Published_Timestamp);
            Assert.Equal("", bp.Slug);
            Assert.Equal("", bp.Title);
            Assert.Equal("", bp.Type_of);
            Assert.Equal("url", bp.Url);
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