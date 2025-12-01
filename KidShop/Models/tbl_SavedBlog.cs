using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("SavedBlogs")]
    public class tbl_SavedBlog
    {
        [Key]
        public int SavedBlogID { get; set; }

        [Required]
        public int UserID { get; set; }
        
        [Required]
        public int BlogID { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [ForeignKey("UserID")]
        public virtual tbl_User? User { get; set; }
        [ForeignKey("BlogID")]
        public virtual tbl_Blog? Blog { get; set; }
    }
}
