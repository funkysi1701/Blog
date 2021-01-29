using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Data
{
    public class GithubService
    {
        private readonly MetricsContext _context;
        private readonly MetricService _service;
        private IConfiguration Configuration { get; set; }

        public GithubService(MetricsContext context, IConfiguration configuration, MetricService MetricService)
        {
            Configuration = configuration;
            _context = context;
            _service = MetricService;
        }

        public async Task GetGitHubFollowers()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await _service.SaveData(user.Followers, 4);
        }

        public async Task GetGitHubFollowing()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await _service.SaveData(user.Following, 5);
        }

        public async Task GetGitHubRepo()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await _service.SaveData(user.PublicRepos, 6);
        }

        public async Task GetGitHubStars()
        {
            var github = GitHub();
            var stars = await github.Activity.Starring.GetAllForCurrent();
            await _service.SaveData(stars.Count, 7);
        }

        public async Task GetCommits()
        {
            var github = GitHub();
            var events = await github.Activity.Events.GetAllUserPerformed(Configuration.GetValue<string>("Username1"));
            var today = events.Where(x => x.Type == "PushEvent" && x.CreatedAt > DateTime.Now.Date).ToList();
            var sofar = _context.Metrics.OrderBy(y => y.Date).ToList();
            sofar = sofar.Where(x => x.Date != null && x.Type == 8 && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
            await _service.SaveData(today.Count + sofar.Last().Value.Value, 8);
        }

        public GitHubClient GitHub()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            return github;
        }
    }
}