using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WebBlog.Data;

namespace WebBlog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        [Inject] private MetricService MetricService { get; set; }
        [Inject] private GithubService GithubService { get; set; }
        [Inject] private NavigationManager UriHelper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Save();
        }

        private async Task Save()
        {
            await GithubService.GetCommits();
            await MetricService.GetTwitterFav();
            await GithubService.GetGitHubStars();
            await GithubService.GetGitHubRepo();
            await MetricService.GetTwitterFollowers();
            await MetricService.GetTwitterFollowing();
            await MetricService.GetNumberOfTweets();
            await GithubService.GetGitHubFollowers();
            await GithubService.GetGitHubFollowing();
            await MetricService.GetDevTo();
            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
