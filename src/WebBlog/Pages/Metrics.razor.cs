using Microsoft.AspNetCore.Components;

namespace WebBlog.Pages
{
    public class MetricsBase : ComponentBase
    {
        [Parameter]
        public int OffSet { get; set; } = 0;

        [Inject] private NavigationManager UriHelper { get; set; }

        protected void PrevDay()
        {
            OffSet--;
            if (OffSet < 0)
            {
                OffSet = 30;
            }
            UriHelper.NavigateTo($"/metrics/{OffSet}", true);
        }

        protected void NextDay()
        {
            OffSet++;
            if (OffSet > 30)
            {
                OffSet = 0;
            }
            UriHelper.NavigateTo($"/metrics/{OffSet}", true);
        }
    }
}
