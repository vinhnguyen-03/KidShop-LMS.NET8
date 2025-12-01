using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SavedBlogStatisticController : Controller
    {
        private DataContext _context;
        public SavedBlogStatisticController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.SavedBlogs
                .Include(s => s.Blog)
                .AsQueryable();

            // Tìm kiếm theo tiêu đề bài viết
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Blog.Title.Contains(search));
            }

            // Thống kê số lần lưu bài viết
            var data = query
                .GroupBy(s => new { s.BlogID, s.Blog.Title })
                .Select(g => new SavedBlogStatisticVM
                {
                    BlogID = g.Key.BlogID,
                    BlogTitle = g.Key.Title,
                    SavedCount = g.Count()
                })
                .OrderByDescending(s => s.SavedCount);

            // Tổng số trang
            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy dữ liệu phân trang
            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền dữ liệu ra ViewBag để tạo phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(pagedData);
        }
    }
}
