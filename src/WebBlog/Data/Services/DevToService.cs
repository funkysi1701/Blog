using System.Linq;
using System.Threading.Tasks;

namespace WebBlog.Data.Services
{
    public class DevToService
    {
        private readonly MetricService _service;
        private BlogService BlogService { get; set; }

        public DevToService(BlogService blogService, MetricService MetricService)
        {
            BlogService = blogService;
            _service = MetricService;
        }

        public async Task GetDevTo()
        {
            var blogs = await BlogService.GetBlogsAsync();
            await _service.SaveData(blogs.Count, 9);
            await _service.SaveData(blogs.Where(x => x.Published).Count(), 10);
            int views = 0;
            int reactions = 0;
            int comments = 0;
            foreach (var item in blogs)
            {
                views += item.Page_Views_Count;
                reactions += item.Positive_Reactions_Count;
                comments += item.Comments_Count;
            }
            await _service.SaveData(views, 11);
            await _service.SaveData(reactions, 12);
            await _service.SaveData(comments, 13);
        }
    }
}
