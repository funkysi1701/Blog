using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlog.Data;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class BlogPostBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }

        protected BlogPosts thisblog;
        protected BlogPostsSingle thisblogsingle;

        [Parameter]
        public string Slug { get; set; }

        protected override async Task OnInitializedAsync()
        {
            List<BlogPosts> blogs = await BlogService.GetBlogsAsync();

            thisblog = blogs.FirstOrDefault(x => x.Slug == Slug && x.Published);
            if (thisblog != null)
            {
                thisblogsingle = await BlogService.GetBlogPostAsync(thisblog.Id);
            }

            this.StateHasChanged();
        }
    }
}
