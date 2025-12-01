using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Enrollment")]
    public class tbl_Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int UserID { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;

        // Mã đơn hàng dùng để chuyển khoản và đối chiếu
        [StringLength(50)]
        public string? OrderCode { get; set; }

        // Tùy chọn: ghi chú hoặc thông tin ngân hàng
        [StringLength(500)]
        public string? PaymentNote { get; set; }
        // true = miễn phí hoặc đã thanh toán
        public bool IsPaid { get; set; } = false;

        [ForeignKey("CourseID")]
        public virtual tbl_Course? Course { get; set; }

        [ForeignKey("UserID")]
        public virtual tbl_User? User { get; set; }
    }
}
