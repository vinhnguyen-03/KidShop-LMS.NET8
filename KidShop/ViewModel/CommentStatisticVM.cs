namespace KidShop.ViewModel
{
    public class CommentStatisticVM
    {
        public long CommentID { get; set; }
        public string? Email { get; set; }
        public string? Contents { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string? TargetType { get; set; } // Product, Blog, Video
    }
}
