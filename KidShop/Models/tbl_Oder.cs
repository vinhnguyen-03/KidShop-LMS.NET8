using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Order")]
    public class tbl_Order
    {
        [Key]
        public int OrderID { get; set; }
        public int UserID { get; set; }
 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        
        [MaxLength(50)]
        public string? Status { get; set; } = "Chờ xử lý";
         
        [MaxLength(100)]
        public string? ReceiverName { get; set; }

        [MaxLength(200)]
        public string? ShippingAddress { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
        [ForeignKey("UserID")]
        public virtual tbl_User? User { get; set; }
        public ICollection<tbl_OrderItem> Items { get; set; }
    }
}
