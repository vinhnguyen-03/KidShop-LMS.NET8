using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("Menu")]
    public class tbl_Menu
    {
        [Key]
        public int MenuID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên menu")]
        [StringLength(100, ErrorMessage = "Tên menu tối đa 100 ký tự")]
        public string? MenuName { get; set; } 
        public int ParentID { get; set; }

        public int Levels { get; set; }

        [Range(0, 100, ErrorMessage = "Thứ tự menu phải từ 0 đến 100")]
        public int MenuOrder { get; set; }

        [Range(0, 10, ErrorMessage = "Vị trí hiển thị không hợp lệ (0–10)")]
        public int Position { get; set; }

        [StringLength(250, ErrorMessage = "Đường dẫn link tối đa 250 ký tự")]
        public string? Link { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public bool? IsActive { get; set; }
    }
}
