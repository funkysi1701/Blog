﻿using System;

namespace WebBlog.Data
{
    public class BlogPosts
    {
        public string Type_of { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Cover_Image { get; set; }
        public bool Published { get; set; }
        public DateTime? Published_At { get; set; }

        //public string Tag_List { get; set; }
        public string Slug { get; set; }

        public string Path { get; set; }
        public string Url { get; set; }
        public string Canonical_Url { get; set; }
        public int Comments_Count { get; set; }
        public int Positive_Reactions_Count { get; set; }
        public int Page_Views_Count { get; set; }
        public DateTime? Published_Timestamp { get; set; }
        public string Body_Html { get; set; }
        public User User { get; set; }
    }
}