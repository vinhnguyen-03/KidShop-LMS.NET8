using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
 [Table("Quiz")]
    public class tbl_Quiz
    {
        [Key]
        public int QuizID { get; set; }

        // Liên kết với bài học (video)
        public int CourseVideoID { get; set; }

        [Required]
        [StringLength(500)]
        public string Question { get; set; } = null!;
 
        // Nếu là trắc nghiệm, lưu đáp án dạng JSON hoặc string
        public string? Options { get; set; }

        // Đáp án đúng (có thể là index hoặc string)
        public string? Answer { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        [ForeignKey("CourseVideoID")]
        public virtual tbl_CourseVideo? Video { get; set; }
    }
}
