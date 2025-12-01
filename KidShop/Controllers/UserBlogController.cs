using KidShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Controllers
{
    [Authorize]
    public class UserBlogController : Controller
    {
        private readonly DataContext _context;

        public UserBlogController(DataContext context)
        {
            _context = context;
        }
        // Giao diện tạo bài viết
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_Blog blog)
        {
            if (!ModelState.IsValid)
                return View(blog);

            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi đăng bài.";
                return RedirectToAction("Login", "User");  
            }

            blog.UserID = int.Parse(userIdClaim.Value);
            blog.CreatedDate = DateTime.Now;
            blog.IsActive = false;  

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bài viết của bạn đã được gửi và đang chờ duyệt.";
            return RedirectToAction("MyBlogs");
        }

        // 👉 Danh sách bài viết của người dùng
        [Authorize]
        public IActionResult MyBlogs()
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
                return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdClaim.Value);

            var blogs = _context.Blogs
                .Where(b => b.UserID == userId)
                .OrderByDescending(b => b.CreatedDate)
                .ToList();

            return View(blogs);
        }
    }
}
