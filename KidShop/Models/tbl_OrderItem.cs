using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("OrderItem")]
    public class tbl_OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderID")]
        public tbl_Order Order { get; set; }

        [ForeignKey("ProductID")]
        public tbl_Product Product { get; set; }
    }
}
