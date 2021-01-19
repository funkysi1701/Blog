using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;

namespace WebBlog.Data
{
    public class MetricService : IMetric
    {
        private readonly MetricsContext _context;
        private TwitterClient UserClient { get; set; }
        private IConfiguration Configuration { get; set; }
        private BlogService BlogService { get; set; }

        public MetricService(MetricsContext context, IConfiguration configuration, BlogService blogService)
        {
            BlogService = blogService;
            Configuration = configuration;
            _context = context;
            UserClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
        }

        public async Task SaveData(int value, int type)
        {
            _context.Add(new Metric
            {
                Id = DateTime.UtcNow.Ticks,
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1"
            });

            await _context.SaveChangesAsync();
        }

        public async Task Delete()
        {
            var m = _context.Metrics.Where(x => x.Value == 0).ToList();
            foreach (var item in m)
            {
                _context.Metrics.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public Metric LoadData(int type, int maxmin)
        {
            try
            {
                if (maxmin == 3)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Value).First();
                }
                else if (maxmin == 2)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Value).First();
                }
                else if (maxmin == 1)
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderByDescending(x => x.Date).First();
                }
                else
                {
                    return _context.Metrics.Where(x => x.Type == type).OrderBy(x => x.Date).First();
                }
            }
            catch
            {
                return new Metric();
            }
        }

        public async Task<List<ChartView>> GetChart(int type)
        {
            var res = await _context.Metrics.Where(x => x.Type == type).ToListAsync();
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

        public async Task GetTwitterFollowers()
        {
            var followers = (await UserClient.Users.GetFollowerIdsAsync(Configuration.GetValue<string>("Username1"))).Length;
            await SaveData(followers, 0);
        }

        public async Task GetTwitterFollowing()
        {
            var friends = (await UserClient.Users.GetFriendIdsAsync(Configuration.GetValue<string>("Username1"))).Length;
            await SaveData(friends, 1);
        }

        public async Task GetNumberOfTweets()
        {
            var friends = await UserClient.Users.GetUserAsync(Configuration.GetValue<string>("Username1"));
            await SaveData(friends.StatusesCount, 2);
        }

        public async Task GetTwitterFav()
        {
            var friends = await UserClient.Users.GetUserAsync(Configuration.GetValue<string>("Username1"));
            await SaveData(friends.FavoritesCount, 3);
        }

        public async Task GetGitHubFollowers()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await SaveData(user.Followers, 4);
        }

        public async Task GetGitHubFollowing()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await SaveData(user.Following, 5);
        }

        public async Task GetGitHubRepo()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await SaveData(user.PublicRepos, 6);
        }

        public async Task GetGitHubStars()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var stars = await github.Activity.Starring.GetAllForCurrent();
            await SaveData(stars.Count, 7);
        }

        public async Task GetCommits()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            var events = await github.Activity.Events.GetAllUserPerformed(Configuration.GetValue<string>("Username1"));
            events = events.Where(x => x.Type == "PushEvent").ToList();
            await SaveData(events.Count, 8);
        }

        public async Task GetDevTo()
        {
            var blogs = await BlogService.GetBlogsAsync();
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
    }
}