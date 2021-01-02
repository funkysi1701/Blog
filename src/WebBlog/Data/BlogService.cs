using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tweetinvi;

namespace WebBlog.Data
{
    public class BlogService
    {
        private HttpClient Client { get; set; }
        private TwitterClient UserClient { get; set; }

        public BlogService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            Client = httpClientFactory.CreateClient("BlogClient");
            Client.DefaultRequestHeaders.Add("api-key", configuration.GetValue<string>("DEVTOAPI"));
            UserClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
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

        public async Task<BlogPosts> GetBlogPostAsync(int id)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/" + id.ToString()));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            BlogPosts posts = JsonConvert.DeserializeObject<BlogPosts>(result);
            httpResponse.Dispose();

            return posts;
        }

        public async Task<int> GetTwitterFollowers()
        {
            var followers = (await UserClient.Users.GetFollowerIdsAsync("funkysi1701")).Length;
            
            return followers;
        }
        public async Task<int> GetTwitterFollowing()
        {
            var friends = (await UserClient.Users.GetFriendIdsAsync("funkysi1701")).Length;
            
            return friends;
        }
    }
}