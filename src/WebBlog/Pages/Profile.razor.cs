using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class ProfileBase : ComponentBase
    {
        [Inject] private MetricService MetricService { get; set; }

        protected override void OnInitialized()
        {
        }

        protected async Task Do()
        {
            await MetricService.SaveProfileData("funkysi1701");
        }
    }
}
