using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Product")]
    public class tbl_Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(350, ErrorMessage = "Tên sản phẩm tối đa 300 ký tự")]
        public string ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không quá 1000 ký tự")]
        public string? Description { get; set; }
        public string? Detail { get; set; }

        [StringLength(550, ErrorMessage = "Link ảnh quá dài (tối đa 550 ký tự)")]
        public string? Image { get; set; }

        [Range(0, 100000000, ErrorMessage = "Giá phải nằm trong khoảng hợp lệ")]
        public decimal? Price { get; set; }

        [Range(0, 100000000, ErrorMessage = "Giá khuyến mãi không hợp lệ")]
        public decimal? PriceSale { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số dương")]
        public int? Quantity { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50, ErrorMessage = "Tên thương hiệu quá dài (tối đa 50 ký tự)")]
        public string? Brand { get; set; }

        [StringLength(155, ErrorMessage = "Tags không quá 155 ký tự")]
        public string? Tags { get; set; }

        public int? Category_ProductID { get; set; }

        public bool? Is_featured { get; set; }

        public bool IsActive { get; set; }
        public ICollection<tbl_ProductImage>? ProductImages { get; set; }
        [ForeignKey("Category_ProductID")]
        public tbl_CategoryProduct? Category { get; set; }

    }
}
