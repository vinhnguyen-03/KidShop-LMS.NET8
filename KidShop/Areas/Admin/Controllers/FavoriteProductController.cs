using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class FavoriteProductController : Controller
    {
        private DataContext _context;
        public FavoriteProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult FavoriteStatistics(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Favorites
                .Include(f => f.Product)
                .AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Product.ProductName.Contains(search));
            }

            // Thống kê số lần yêu thích
            var data = query
                .GroupBy(f => new { f.ProductID, f.Product.ProductName })
                .Select(g => new FavoriteStatisticVM
                {
                    ProductID = g.Key.ProductID,
                    ProductName = g.Key.ProductName,
                    FavoriteCount = g.Count()
                })
                .OrderByDescending(f => f.FavoriteCount);

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
