using KidShop.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<tbl_Menu> Menus { get; set; }
        public DbSet<tbl_CategoryProduct> CategorieProducts { get; set; }
        public DbSet<tbl_Banner> Banners { get; set; }
        public DbSet<tbl_Introduce> Introduces { get; set; }
        public DbSet<tbl_Blog> Blogs { get; set; }
        public DbSet<tbl_SavedBlog> SavedBlogs { get; set; }
        public DbSet<tbl_Video> Videos { get; set; }
        public DbSet<tbl_CategoryVideo> CategoryVideos { get; set; }
        public DbSet<tbl_Course> Courses { get; set; }
        public DbSet<tbl_Lecturer> Lecturers { get; set; }
        public DbSet<tbl_CourseVideo> CourseVideos { get; set; }
        public DbSet<tbl_Quiz> Quizzes { get; set; }
        public DbSet<tbl_Enrollment> Enrollments { get; set; }
        public DbSet<tbl_Comment> Comments { get; set; }
        public DbSet<tbl_Product> Products { get; set; }
        public DbSet<tbl_ProductImage> ProductImages { get; set; }
        public DbSet<tbl_Cart> Carts { get; set; }
        public DbSet<tbl_CartItem> CartItems { get; set; }
        public DbSet<tbl_Favorites> Favorites { get; set; }
        public DbSet<tbl_Order> Orders { get; set; }
        public DbSet<tbl_OrderItem> OrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<tbl_User> Users { get; set; }
        public DbSet<AdminMenu> AdminMenus { get; set; }
    }
}
