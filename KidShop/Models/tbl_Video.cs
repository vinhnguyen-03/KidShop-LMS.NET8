using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Video")]
    public class tbl_Video
    {
        [Key]
        public int VideoID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề video")]
        [StringLength(550, ErrorMessage = "Tiêu đề tối đa 550 ký tự")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Describe { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thời lượng phải là số không âm (giây)")]
        public int? Duration { get; set; }

        [StringLength(450, ErrorMessage = "Link video quá dài (tối đa 450 ký tự)")]
        public string? Link { get; set; }

        [StringLength(450, ErrorMessage = "Đường dẫn ảnh quá dài (tối đa 450 ký tự)")]
        public string? Image { get; set; }
        public int ViewCount { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }
        public int? Category_VideoID { get; set; }
        public bool? IsFeatured { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("Category_VideoID")]
        public tbl_CategoryVideo? CategoryVideo { get; set; }
        // Thuộc tính đổi ss sang định dạng hh:mm:ss hoặc mm:ss
        [NotMapped]
        public string? DurationDisplay
            {
                get
                {
                    if (Duration == null) return "";

                    var t = TimeSpan.FromSeconds(Duration.Value);

                    // Nếu >= 1 giờ -> hh:mm:ss, nếu < 1 giờ -> mm:ss
                    return t.Hours > 0
                        ? t.ToString(@"hh\:mm\:ss")
                        : t.ToString(@"mm\:ss");
                }
            }
    }
}
