using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CommentStatisticsController : Controller
    {
        private readonly DataContext _context;

        public CommentStatisticsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? type, string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Comments
                .Include(c => c.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(f => f.User.Email.Contains(search));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(c => c.TargetType == type);

            var commentsQuery = query
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new CommentStatisticVM
                {
                    CommentID = c.CommentID,
                    Email = c.User != null ? c.User.Email : "Ẩn danh",
                    Contents = c.Contents,
                    CreatedDate = c.CreatedDate,
                    IsActive = c.IsActive,
                    TargetType = c.TargetType
                });

            // Tổng số trang
            int totalItems = commentsQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedData = commentsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.SelectedType = type;

            ViewBag.TotalProduct = _context.Comments.Count(c => c.TargetType == "Product");
            ViewBag.TotalBlog = _context.Comments.Count(c => c.TargetType == "Blog");
            ViewBag.TotalVideo = _context.Comments.Count(c => c.TargetType == "Video");
            ViewBag.TotalCourse = _context.Comments.Count(c => c.TargetType == "Course");

            return View(pagedData); // <--- Đây mới là dữ liệu phân trang
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
                return NotFound();

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🗑️ Xóa bình luận thành công!";
            return RedirectToAction("Index");
        }
    }
}
