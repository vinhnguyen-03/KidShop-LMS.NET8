using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Favorites")]
    public class tbl_Favorites
    {
        [Key]
        public int FavoriteID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ProductID))]
        public virtual tbl_Product? Product { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual tbl_User? User { get; set; }
    }
}
