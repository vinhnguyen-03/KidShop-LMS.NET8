using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Banner")]
    public class tbl_Banner
    {
        [Key]
        public int BannerID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập slogan")]
        [StringLength(150, ErrorMessage = "Slogan tối đa 150 ký tự")]
        public string? Slogan { get; set; }

        [StringLength(150, ErrorMessage = "Tagline tối đa 150 ký tự")]
        public string? Tagline { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ảnh banner")]
        [StringLength(300, ErrorMessage = "Link ảnh quá dài (tối đa 300 ký tự)")]
        public string? Image { get; set; }

        [StringLength(150, ErrorMessage = "Đường dẫn link quá dài (tối đa 150 ký tự)")]
        public string? Link { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool? IsActive { get; set; }

    }
}
