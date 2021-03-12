using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlog.Data;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class RedirectBase : ComponentBase
    {
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private BlogService BlogService { get; set; }

        protected List<BlogPosts> blogs;

        [Parameter]
        public string Year { get; set; }

        [Parameter]
        public string Month { get; set; }

        [Parameter]
        public string Day { get; set; }

        [Parameter]
        public string Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
            blogs = await BlogService.GetBlogsAsync();
            var url = HttpContextAccessor.HttpContext.Request.Path.Value;
            foreach (var item in blogs.Where(x => x.Canonical_Url.Contains("https://www.funkysi1701.com")))
            {
                if (item.Canonical_Url.Contains(url))
                {
                    HttpContextAccessor.HttpContext.Response.Redirect("/posts/" + item.Slug);
                }
            }
        }
    }
}
