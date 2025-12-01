using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Blog")]
    public class tbl_Blog
    {
        [Key]
        public int BlogID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [MaxLength(300, ErrorMessage = "Tiêu đề tối đa 300 ký tự")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "Tóm tắt tối đa 500 ký tự")]
        public string? Abstract { get; set; }

        [ValidateNever] //  Không validate, cho phép chứa HTML
        public string? Contents { get; set; }

        [ValidateNever] //  
        public string? Image { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        // so lượt xem bài viet
        public int ViewCount { get; set; } = 0;
         
        public bool IsActive { get; set; } = false; // Mặc định chưa duyệt

        public int UserID { get; set; } //  
        [ForeignKey("UserID")]
        public virtual tbl_User? User { get; set; }  
    }
}
