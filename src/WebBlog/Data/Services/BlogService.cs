using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlog.Data.Services
{
    public class BlogService
    {
        private HttpClient Client { get; set; }

        public BlogService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            Client = httpClientFactory.CreateClient("BlogClient");
            Client.DefaultRequestHeaders.Add("api-key", configuration.GetValue<string>("DEVTOAPI"));
        }

        public async Task<List<BlogPosts>> GetBlogsAsync()
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/me/all?per_page=200"));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            List<BlogPosts> posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            httpResponse.Dispose();

            return posts;
        }

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/" + id.ToString()));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            BlogPostsSingle post = JsonConvert.DeserializeObject<BlogPostsSingle>(result);
            httpResponse.Dispose();

            return post;
        }
    }
}
