using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("CategoryProduct")]
    public class tbl_CategoryProduct
    {
        [Key]
        public int Category_ProductID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(250, ErrorMessage = "Tên danh mục tối đa 250 ký tự")]
        public string? CategoryName { get; set; }

        [StringLength(250, ErrorMessage = "Mô tả tối đa 250 ký tự")]
        public string? Describe { get; set; }

        [StringLength(350, ErrorMessage = "Đường dẫn hình ảnh tối đa 350 ký tự")]
        public string? Image { get; set; }

        [StringLength(150, ErrorMessage = "Liên kết tối đa 150 ký tự")]
        public string? Link { get; set; }

        public bool IsActive { get; set; }
        public ICollection<tbl_Product>? Products { get; set; }
    }
}
