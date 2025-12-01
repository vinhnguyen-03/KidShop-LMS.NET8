using KidShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidShop.Controllers
{
    [Authorize]
    public class SaveBlogController : Controller
    {
        private readonly DataContext _context;
        public SaveBlogController(DataContext context)
        {
            _context = context;
        }
        // Lấy UserID từ Claims (Identity)
        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserID");
            if (claim == null) return null;

            if (int.TryParse(claim.Value, out int userId))
                return userId;
            return null;
        }

        // ===== Xem danh sách bài viết đã lưu =====
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var savedBlogs = await _context.SavedBlogs
                .Include(s => s.Blog)
                .Where(s => s.UserID == userId)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return View(savedBlogs);
        }

        // ===== Thêm bài viết vào danh sách đã lưu =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int blogId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            bool exists = await _context.SavedBlogs
                .AnyAsync(s => s.UserID == userId && s.BlogID == blogId);

            if (!exists)
            {
                _context.SavedBlogs.Add(new tbl_SavedBlog
                {
                    UserID = userId.Value,
                    BlogID = blogId,
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["Message"] = "💾 Đã lưu bài viết!";
            }

            // Redirect về trang chi tiết bài viết
            return RedirectToAction("BlogDetail", "Blog", new { id = blogId });
        }

        // ===== Xóa bài viết khỏi danh sách đã lưu =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int blogId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var item = await _context.SavedBlogs
                .FirstOrDefaultAsync(s => s.UserID == userId && s.BlogID == blogId);

            if (item != null)
            {
                _context.SavedBlogs.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Message"] = "❌ Đã xóa bài viết khỏi danh sách lưu!";
            }

            // Redirect về trang chi tiết bài viết
            return RedirectToAction("BlogDetail", "Blog", new { id = blogId });
        }

        // ===== Kiểm tra bài viết đã lưu hay chưa =====
        public bool IsSaved(int blogId)
        {
            var userId = GetUserId();
            if (userId == null) return false;

            return _context.SavedBlogs.Any(s => s.UserID == userId && s.BlogID == blogId);
        }
    }
}
