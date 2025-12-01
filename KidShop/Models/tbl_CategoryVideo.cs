using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("CategoryVideo")]
    public class tbl_CategoryVideo
    {
        [Key]
        public int Category_VideoID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề danh mục video")]
        [StringLength(250, ErrorMessage = "Tiêu đề tối đa 250 ký tự")]
        public string? Title { get; set; }

        [StringLength(150, ErrorMessage = "Liên kết tối đa 150 ký tự")]
        public string? Link { get; set; }

        [StringLength(250, ErrorMessage = "Mô tả tối đa 250 ký tự")]
        public string? Describe { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool? IsActive { get; set; }
        public ICollection<tbl_CategoryVideo>? CategoryVideos { get; set; }
    }
}
