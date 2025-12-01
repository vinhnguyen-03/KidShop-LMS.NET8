using KidShop.Models;

namespace KidShop.ViewModel.Blog
{
    public class BlogStatsVM
    {
        // thống kê ở Admin về blog
        public int TotalBlogs { get; set; }

        public tbl_Blog? MostViewedBlog { get; set; }

        public string? TopUserName { get; set; }
        public int TopUserBlogCount { get; set; }
    }
}
