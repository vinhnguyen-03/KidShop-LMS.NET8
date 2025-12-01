using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Introduce")]
    public class tbl_Introduce
    {
        [Key]
        public int IntroduceID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề giới thiệu")]
        [StringLength(250, ErrorMessage = "Tiêu đề tối đa 250 ký tự")]
        public string? Title { get; set; }

        [StringLength(250, ErrorMessage = "Mô tả tối đa 250 ký tự")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Tên icon tối đa 50 ký tự")]
        public string? Icon { get; set; }

        [StringLength(20, ErrorMessage = "Mã màu nền tối đa 20 ký tự")]
        public string? Background_color { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool? IsActive { get; set; }
    }
}
