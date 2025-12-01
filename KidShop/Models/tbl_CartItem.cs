using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("CartItem")]
    public class tbl_CartItem
    {
        [Key]
        public int CartItemID { get; set; }
        public int CartID { get; set; }

        public int ProductID { get; set; }
        public int Quantity { get; set; }

        // Khóa ngoại và navigation
        [ForeignKey(nameof(CartID))]
        public virtual tbl_Cart? Cart { get; set; }

        [ForeignKey(nameof(ProductID))]
        public virtual tbl_Product? Product { get; set; }
    }
}
