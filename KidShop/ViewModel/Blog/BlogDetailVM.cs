using KidShop.Models;

namespace KidShop.ViewModel.Blog
{
    public class BlogDetailVM
    {
        public tbl_Blog Blog { get; set; }
        public bool BlogIsSaved { get; set; } = false;
        public List<CommentWithUserVM> Comments { get; set; } = new List<CommentWithUserVM>();
    }
}
