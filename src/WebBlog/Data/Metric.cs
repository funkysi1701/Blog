﻿namespace WebBlog.Data
{
    public class Metric
    {
        public long Id { get; set; }
        public int? Value { get; set; }
        public int Type { get; set; }
        public string PartitionKey { get; set; }
    }
}
