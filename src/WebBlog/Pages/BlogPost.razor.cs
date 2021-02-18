using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlog.Data;

namespace WebBlog.Pages
{
    public class BlogPostBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        private List<BlogPosts> blogs;
        protected BlogPosts thisblog;
        protected BlogPostsSingle thisblogsingle;

        [Parameter]
        public string Slug { get; set; }

        protected override async Task OnInitializedAsync()
        {
            blogs = await BlogService.GetBlogsAsync();

            thisblog = blogs.Where(x => x.Slug == Slug && x.Published).FirstOrDefault();
            if (thisblog != null)
            {
                thisblogsingle = await BlogService.GetBlogPostAsync(thisblog.Id);
            }

            this.StateHasChanged();
        }
    }
}
