namespace WebBlog.Data
{
    public class Profile
    {
        public long Id { get; set; }
        public string PartitionKey { get; set; }
        public string UserName { get; set; }
    }
}
