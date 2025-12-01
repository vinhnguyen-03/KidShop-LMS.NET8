namespace KidShop.ViewModel
{
    public class CommentWithUserVM
    {
        public long CommentID { get; set; }
        public string? Contents { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? FullName { get; set; }
        public int? UserID { get; set; }
        public int? TargetID { get; set; }
        public string? TargetType { get; set; }
        public int? ParentID { get; set; }
    }
}
