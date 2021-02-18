using Microsoft.AspNetCore.Components;
using WebBlog.Data;

namespace WebBlog.Pages
{
    public class MetricsBase : ComponentBase
    {
        [Inject] MetricService MetricService { get; set; }
        protected Metric followers = new();
        protected Metric following = new();
        protected Metric numtweets = new();
        protected Metric ghfollowers = new();
        protected Metric ghfollowing = new();
        protected Metric ghrepo = new();
        protected Metric ghstars = new();
        protected Metric commits = new();
        protected Metric fav = new();
        protected Metric views = new();
        protected Metric reactions = new();
        protected Metric comments = new();
        protected Metric pubposts = new();
        protected Metric posts = new();

        protected override void OnInitialized()
        {
            Load();
        }

        private void Load()
        {
            followers = MetricService.LoadData(0, 1);
            following = MetricService.LoadData(1, 1);
            numtweets = MetricService.LoadData(2, 1);
            fav = MetricService.LoadData(3, 1);
            ghfollowers = MetricService.LoadData(4, 1);
            ghfollowing = MetricService.LoadData(5, 1);
            ghrepo = MetricService.LoadData(6, 1);
            ghstars = MetricService.LoadData(7, 1);
            commits = MetricService.LoadData(8, 1);
            posts = MetricService.LoadData(9, 1);
            pubposts = MetricService.LoadData(10, 1);
            views = MetricService.LoadData(11, 1);
            reactions = MetricService.LoadData(12, 1);
            comments = MetricService.LoadData(13, 1);
        }
    }
}
