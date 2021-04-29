using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
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
            var r = new Random();
            var rnd = r.Next(2);
            if (rnd == 1)
            {
                await PowerService.GetElec();
            }
            else
            {
                await PowerService.GetGas();
            }

            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
