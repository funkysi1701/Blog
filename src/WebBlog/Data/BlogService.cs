﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebBlog.Data
{
    public class BlogService
    {
        private HttpClient Client { get; set; }

        public BlogService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            Client = httpClientFactory.CreateClient("BlogClient");
            Client.DefaultRequestHeaders.Add("api-key", configuration.GetValue<string>("DEVTOAPI"));
        }

        public async Task<List<BlogPosts>> GetBlogsAsync()
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/me?per_page=100"));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            List<BlogPosts> posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            httpResponse.Dispose();

            return posts;
        }

        public async Task<List<BlogPosts>> GetBlogsAsync(int start, int count)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/me?per_page=100"));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            List<BlogPosts> posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            httpResponse.Dispose();
            if (posts[start] == null)
            {
                return posts.OrderByDescending(x => x.Published_At).ToList();
            }
            else if(posts.OrderByDescending(x => x.Published_At).Where(y => y.Published_At <= posts[start].Published_At).Count() > count)
            {
                return posts.OrderByDescending(x => x.Published_At).Where(y => y.Published_At <= posts[start].Published_At).Take(count).ToList();
            }
            else return posts.OrderByDescending(x => x.Published_At).Where(y => y.Published_At <= posts[start].Published_At).ToList();
        }

        public async Task<BlogPosts> GetBlogPostAsync(int id)
        {
            var call = Client.GetAsync(new Uri(Client.BaseAddress + "articles/" + id.ToString()));
            HttpResponseMessage httpResponse = await call;

            string result = await httpResponse.Content.ReadAsStringAsync();
            BlogPosts posts = JsonConvert.DeserializeObject<BlogPosts>(result);
            httpResponse.Dispose();

            return posts;
        }
    }
}