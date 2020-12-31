using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Data;

namespace WebBlog
{
    public class SitemapMiddleware
    {
        private RequestDelegate _next;
        private BlogService _blogService;
        public SitemapMiddleware(RequestDelegate next, BlogService BlogService)
        {
            _next = next;
            _blogService = BlogService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/xml";
                string sitemapContent = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";
                
                var blogs = await _blogService.GetBlogsAsync();
                foreach (var blog in blogs)
                {
                        sitemapContent += "<url>";
                        sitemapContent += string.Format("<loc>{0}</loc>", blog.Canonical_Url);
                        sitemapContent += string.Format("<lastmod>{0}</lastmod>", blog.Published_At.ToString("yyyy-MM-dd"));
                        sitemapContent += "</url>";
                    
                }
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/about");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/pwned-pass");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/config");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "</urlset>";
                using var memoryStream = new MemoryStream();
                var bytes = Encoding.UTF8.GetBytes(sitemapContent);
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream, bytes.Length);
            }
        }
    }
}
