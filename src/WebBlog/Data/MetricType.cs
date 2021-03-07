using System.ComponentModel;

namespace WebBlog.Data
{
    public enum MetricType
    {
        [Description("Twitter Followers")]
        TwitterFollowers,

        [Description("Twitter Following")]
        TwitterFollowing,

        [Description("Number Of Tweets")]
        NumberOfTweets,

        [Description("Twitter Favourites")]
        TwitterFavourites,

        [Description("GitHub Followers")]
        GitHubFollowers,

        [Description("GitHub Following")]
        GitHubFollowing,

        [Description("GitHub Repo")]
        GitHubRepo,

        [Description("GitHub Stars")]
        GitHubStars,

        [Description("GitHub Commits")]
        GitHubCommits,

        [Description("DevTo Posts")]
        DevToPosts,

        [Description("DevTo Published Posts")]
        DevToPublishedPosts,

        [Description("DevTo Views")]
        DevToViews,

        [Description("DevTo Reactions")]
        DevToReactions,

        [Description("DevTo Comments")]
        DevToComments,

        [Description("Gas (m^3)")]
        Gas,

        [Description("Electricity (kW/h)")]
        Electricity,
    }
}
