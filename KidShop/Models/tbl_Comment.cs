using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Comment")]
    public class tbl_Comment
    {
        [Key]
        public long CommentID { get; set; }

        [Required(ErrorMessage = "Thiếu thông tin người dùng")]
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Thiếu đối tượng bình luận (sản phẩm hoặc bài viết)")]
        public int? TargetID { get; set; } // ProductID hoặc BlogID

        [Required(ErrorMessage = "Thiếu loại đối tượng bình luận")]
        [StringLength(50, ErrorMessage = "Loại bình luận tối đa 50 ký tự")]
        public string? TargetType { get; set; } // "Product" hoặc "Blog"

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận")]
        [StringLength(800, ErrorMessage = "Nội dung bình luận tối đa 800 ký tự")]
        public string? Contents { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public int? ParentID { get; set; } // Dành cho bình luận trả lời

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool IsActive { get; set; } 

        [ForeignKey("UserID")]
        public virtual tbl_User? User { get; set; }

    }
}
