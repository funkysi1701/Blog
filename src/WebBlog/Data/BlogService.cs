using Microsoft.EntityFrameworkCore;
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

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/" + id.ToString()));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            BlogPostsSingle post = JsonConvert.DeserializeObject<BlogPostsSingle>(result);
            httpResponse.Dispose();

            return post;
        }

        public async Task GetTwitterFollowers()
        {
            var followers = (await UserClient.Users.GetFollowerIdsAsync("funkysi1701")).Length;
            await SaveData(followers, 0);
        }

        public async Task GetTwitterFollowing()
        {
            var friends = (await UserClient.Users.GetFriendIdsAsync("funkysi1701")).Length;
            await SaveData(friends, 1);
        }

        public async Task GetNumberOfTweets()
        {
            var friends = await UserClient.Users.GetUserAsync("funkysi1701");
            await SaveData(friends.StatusesCount, 2);
        }

        public async Task GetTwitterFav()
        {
            var friends = await UserClient.Users.GetUserAsync("funkysi1701");
            await SaveData(friends.FavoritesCount, 3);
        }

        public async Task GetGitHubFollowers()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            await SaveData(user.Followers, 4);
        }

        public async Task GetGitHubFollowing()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            await SaveData(user.Following, 5);
        }

        public async Task GetGitHubRepo()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var user = await github.User.Get("funkysi1701");
            await SaveData(user.PublicRepos, 6);
        }

        public async Task GetGitHubStars()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var stars = await github.Activity.Starring.GetAllForCurrent();
            await SaveData(stars.Count, 7);
        }

        public async Task GetCommits()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var events = await github.Activity.Events.GetAllUserPerformed("funkysi1701");
            events = events.Where(x => x.Type == "PushEvent").ToList();
            await SaveData(events.Count, 8);
        }

        public async Task GetDevTo()
        {
            var blogs = await GetBlogsAsync();
            await SaveData(blogs.Count, 9);
            await SaveData(blogs.Where(x => x.Published).Count(), 10);
            int views = 0;
            int reactions = 0;
            int comments = 0;
            foreach (var item in blogs)
            {
                views += item.Page_Views_Count;
                reactions += item.Positive_Reactions_Count;
                comments += item.Comments_Count;
            }
            await SaveData(views, 11);
            await SaveData(reactions, 12);
            await SaveData(comments, 13);
        }

        public async Task SaveData(int value, int type)
        {
            using var context = new MetricsContext(Configuration);

            context.Add(new Metric
            {
                Id = DateTime.UtcNow.Ticks,
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1"
            });

            await context.SaveChangesAsync();
        }

        public Metric LoadData(int type, int maxmin)
        {
            using var context = new MetricsContext(Configuration);
            try
            {
                if (maxmin == 3)
                {
                    return context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Value).First();
                }
                else if (maxmin == 2)
                {
                    return context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Value).First();
                }
                else if (maxmin == 1)
                {
                    return context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Date).First();
                }
                else
                {
                    return context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Date).First();
                }
            }
            catch
            {
                return new Metric();
            }
        }

        public async Task<List<ChartView>> GetChart(int type)
        {
            using var context = new MetricsContext(Configuration);
            var res = await context.Metrics.Where(x => x.Type == type).ToListAsync();
            var result = new List<ChartView>();
            foreach (var item in res.Where(x => x.Date != null))
            {
                var c = new ChartView
                {
                    Date = item.Date.Value.Year.ToString("D4") + "-" + item.Date.Value.Month.ToString("D2") + "-" + item.Date.Value.Day.ToString("D2"),
                    Total = item.Value
                };
                result.Add(c);
            }
            return result;
        }
    }
}