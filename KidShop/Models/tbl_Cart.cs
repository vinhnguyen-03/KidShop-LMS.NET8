using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Cart")]
    public class tbl_Cart
    {
        [Key]
        public int CartID { get; set; }
 
        [ForeignKey("User")]
        public int UserID { get; set; }

        public tbl_User User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<tbl_CartItem> Items { get; set; } = new List<tbl_CartItem>();
    }
    
}
