using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("CourseVideo")]
    public class tbl_CourseVideo
    {
        [Key]
        public int CourseVideoID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề video")]
        [StringLength(550)]
        public string? Title { get; set; } 

        [Required(ErrorMessage = "Vui lòng nhập link YouTube")]
        [StringLength(500)]
        public string? Link { get; set; } // Link YouTube

        public int? Duration { get; set; } // Thời lượng (phút, nếu có)

        public int? OrderIndex { get; set; } // Thứ tự trong khóa học

        public int CourseID { get; set; }

        [ForeignKey(nameof(CourseID))]
        public virtual tbl_Course? Course { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public bool IsPreview { get; set; }
        public bool IsActive { get; set; }  
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
        [NotMapped]
        public bool IsYouTube
        {
            get => !string.IsNullOrEmpty(Link) && (Link.Contains("youtube.com") || Link.Contains("youtu.be"));
        }
    }
}
