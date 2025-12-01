using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductStatisticsController : Controller
    {
        private readonly DataContext _context;

        public ProductStatisticsController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            int lowStockThreshold = 5;

            // ====== THỐNG KÊ TỒN KHO ======
            var totalProducts = _context.Products.Count();

            var outOfStock = _context.Products
                .Where(p => p.Quantity <= 0 || p.Quantity == null)
                .Count();

            var lowStock = _context.Products
                .Where(p => p.Quantity > 0 && p.Quantity < lowStockThreshold)
                .Count();

            // ====== DANH SÁCH HẾT HÀNG ======
            var outOfStockProducts = _context.Products
                .Where(p => p.Quantity <= 0 || p.Quantity == null)
                .Select(p => new ProductItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Quantity = p.Quantity
                })
                .ToList();

            // ====== DANH SÁCH SẮP HẾT HÀNG ======
            var lowStockProducts = _context.Products
                .Where(p => p.Quantity > 0 && p.Quantity < lowStockThreshold)
                .Select(p => new ProductItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Quantity = p.Quantity
                })
                .ToList();

            // ====== TOP 5 SẢN PHẨM BÁN CHẠY 1 THÁNG ======
            var lastMonth = DateTime.Now.AddMonths(-1);

            var top5Products = _context.OrderItems
                .Where(i => i.Order.CreatedAt >= lastMonth)
                .GroupBy(i => i.ProductID)
                .Select(g => new TopProductItem
                {
                    ProductID = g.Key,
                    ProductName = g.First().Product.ProductName,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();


            // ====== SẢN PHẨM BÁN CHẬM 3 THÁNG ======
            var last3Months = DateTime.Now.AddMonths(-3);

            var slowProducts = _context.Products
                .GroupJoin(
                    _context.OrderItems
                        .Where(i => i.Order.CreatedAt >= last3Months),
                    p => p.ProductID,
                    i => i.ProductID,
                    (p, orderItems) => new
                    {
                        Product = p,
                        TotalSold = orderItems.Sum(x => (int?)x.Quantity) ?? 0
                    }
                )
                .Where(x => x.TotalSold == 0)  // không bán được trong 3 tháng
                .Select(x => new SlowProductItem
                {
                    ProductID = x.Product.ProductID,
                    ProductName = x.Product.ProductName,
                    Stock = x.Product.Quantity
                })
                .ToList();


            // ====== GOM TẤT CẢ VÀO 1 MODEL ======
            var vm = new ProductStatisticsVM
            {
                TotalProducts = totalProducts,
                OutOfStock = outOfStock,
                LowStock = lowStock,
                OutOfStockProducts = outOfStockProducts,
                LowStockProducts = lowStockProducts,
                Top5Products = top5Products,
                SlowProducts = slowProducts
            };

            return View(vm);
        }
    }
}
