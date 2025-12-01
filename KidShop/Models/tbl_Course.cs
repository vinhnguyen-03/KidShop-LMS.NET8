using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Course")]
    public class tbl_Course
    {
        [Key]
        public int CourseID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề khóa học")]
        [StringLength(350, ErrorMessage = "Tiêu đề tối đa 350 ký tự")]
        public string? CourseName { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Describe { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Contents { get; set; }

        [StringLength(550, ErrorMessage = "hình ảnh tối đa 550 ký tự")]
        public string? Image { get; set; }
        [Range(0, 100000000, ErrorMessage = "Giá phải nằm trong khoảng hợp lệ")]
        public decimal? Price { get; set; }

        [Range(0, 100000000, ErrorMessage = "Giá khuyến mãi không hợp lệ")]
        public decimal? PriceSale { get; set; }
        
        public int LecturerID { get; set; }
      
        public string? Tag { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        //  khóa học miễn phí hay mất phí
        public bool IsFree { get; set; } = true;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool IsActive { get; set; }


        [ForeignKey("LecturerID")]
        public virtual tbl_Lecturer? Lecturers { get; set; }
        public virtual ICollection<tbl_CourseVideo>? CourseVideos { get; set; }
      
        public virtual ICollection<tbl_Enrollment>? Enrollments { get; set; }
    }
}
