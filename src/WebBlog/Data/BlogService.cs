using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tweetinvi;

namespace WebBlog.Data
{
    public class BlogService
    {
        private HttpClient Client { get; set; }
        private TwitterClient UserClient { get; set; }
        private IConfiguration Configuration { get; set; }

        public BlogService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            Configuration = configuration;
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

        public async Task<int> GetNumberOfTweets()
        {
            var friends = await UserClient.Users.GetUserAsync("funkysi1701");

            return friends.StatusesCount;
        }

        public async Task<int> GetTwitterFav()
        {
            var friends = await UserClient.Users.GetUserAsync("funkysi1701");

            return friends.FavoritesCount;
        }

        public async Task<int> GetGitHubFollowers()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            return user.Followers;
        }

        public async Task<int> GetGitHubFollowing()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            return user.Following;
        }

        public async Task<int> GetGitHubRepo()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            return user.PublicRepos;
        }

        public async Task<int> GetGitHubStars()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var stars = await github.Activity.Starring.GetAllForCurrent();   
            
            return stars.Count;
        }

        public async Task<int> GetCommits()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var events = await github.Activity.Events.GetAllUserPerformed("funkysi1701");
            events = events.Where(x => x.Type == "PushEvent").ToList();
            return events.Count;
        }
    }
}