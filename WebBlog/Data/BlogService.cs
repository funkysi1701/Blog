using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBlog.Data
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
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/me?per_page=100"));
            HttpResponseMessage httpResponse = await call.ConfigureAwait(false);

            string result = httpResponse.Content.ReadAsStringAsync().Result;
            List<BlogPosts> posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            httpResponse.Dispose();

            return posts;
        }

        public async Task<BlogPosts> GetBlogPostAsync(int id)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/" + id.ToString()));
            HttpResponseMessage httpResponse = await call.ConfigureAwait(false);

            string result = httpResponse.Content.ReadAsStringAsync().Result;
            BlogPosts posts = JsonConvert.DeserializeObject<BlogPosts>(result);
            httpResponse.Dispose();

            return posts;
        }
    }
}