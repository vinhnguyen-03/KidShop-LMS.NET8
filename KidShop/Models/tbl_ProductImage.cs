using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("ProductImage")]
    public class tbl_ProductImage
    {
        [Key]
        public int ProductImageID { get; set; }
        public int ProductID { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltText { get; set; }
        // Thêm navigation
        [ForeignKey("ProductID")]
        public tbl_Product? Product { get; set; }
    }
}
