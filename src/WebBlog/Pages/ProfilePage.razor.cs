using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBlog.Data;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class ProfileBase : ComponentBase
    {
        [Inject] private MetricService MetricService { get; set; }

        protected List<Profile> Profiles;

        protected override async Task OnInitializedAsync()
        {
            Profiles = await MetricService.GetProfiles();
        }

        protected async Task Do()
        {
            await MetricService.SaveProfileData("funkysi1701");
            StateHasChanged();
        }
    }
}
