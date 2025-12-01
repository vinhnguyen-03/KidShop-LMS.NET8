using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Lecturer")]
    public class tbl_Lecturer
    {
        [Key]
        public int LecturerID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên giảng viên")]
        [StringLength(150, ErrorMessage = "Tên giảng viên tối đa 150 ký tự")]
        public string? LecturerName { get; set; }
        [StringLength(150, ErrorMessage = "ảnh tối đa 150 ký tự")]
        public string? Avatar { get; set; }
        [StringLength(20, ErrorMessage = "phone tối đa 20 ký tự")]
        public string? Phone { get; set; }
        [StringLength(150, ErrorMessage = "email tối đa 150 ký tự")]
        public string? Email { get; set; }
        public string? Experience { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool IsActive { get; set; }
        public virtual ICollection<tbl_Course>? Courses { get; set; }
    }
}
