using KidShop.Models;

namespace KidShop.ViewModel
{
    public class VideoDetailVM
    {
        public tbl_Video Video { get; set; }
        public List<CommentWithUserVM> Comments { get; set; } = new List<CommentWithUserVM>();
        public List<tbl_Video> RelatedVideos { get; set; } = new List<tbl_Video>();
    }
}
