using System;

namespace WebBlog.Components
{
    public static class PageTitleGenerator
    {
        /// <summary>
        /// Create Page Title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

            title = title.Replace('-', ' ');
            if (title.Contains("posts"))
            {
                title = title.Replace("posts", "");
                title = title.Substring(0, title.Length - 4);
            }

            string pageTitle = title switch
            {
                "/" => string.Empty,
                _ => $"{title}",
            };
            return pageTitle + " - Funky Si's Tech Talk";
        }
    }
}