using System;

namespace WebBlog.Data
{
    public class Metric
    {
        public long Id { get; set; }
        public string PartitionKey { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
        public int? Type { get; set; }
    }
}
