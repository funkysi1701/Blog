using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WebBlog.Data;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        [Inject] private GithubService GithubService { get; set; }
        [Inject] private DevToService DevToService { get; set; }
        [Inject] private TwitterService TwitterService { get; set; }
        [Inject] private PowerService PowerService { get; set; }
        [Inject] private NavigationManager UriHelper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Save();
        }

        private async Task Save()
        {
            await GithubService.GetCommits();
            await TwitterService.GetTwitterFav();
            await GithubService.GetGitHubStars();
            await GithubService.GetGitHubRepo();
            await TwitterService.GetTwitterFollowers();
            await TwitterService.GetTwitterFollowing();
            await TwitterService.GetNumberOfTweets();
            await GithubService.GetGitHubFollowers();
            await GithubService.GetGitHubFollowing();
            await DevToService.GetDevTo();
            await PowerService.GetElec();
            await PowerService.GetGas();
            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
