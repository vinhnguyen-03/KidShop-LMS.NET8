using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class RevenueProductController : Controller
    {
        private readonly DataContext _context;
        public RevenueProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new RevenueProductVM();

            // ====== Tổng quan ======
            model.TotalRevenue = _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            model.TotalCost = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0)
                .Sum(t => -(t.QuantityChange) * (t.CostPrice ?? 0));

            model.TotalProfit = model.TotalRevenue - model.TotalCost;

            model.TotalOrders = _context.Orders.Count();
            model.TotalSoldProducts = _context.OrderItems.Sum(i => (int?)i.Quantity) ?? 0;

            // ====== Hôm nay ======
            var today = DateTime.Today;

            model.RevenueToday = _context.Orders
                .Where(o => o.Status == "Đã thanh toán" && o.CreatedAt.Date == today)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            model.CostToday = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0 && t.CreatedDate.Date == today)
                .Sum(t => -(t.QuantityChange) * (t.CostPrice ?? 0));

            model.ProfitToday = model.RevenueToday - model.CostToday;

            // ====== Tháng này ======
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            model.RevenueThisMonth = _context.Orders
                .Where(o => o.Status == "Đã thanh toán" &&
                            o.CreatedAt.Month == month &&
                            o.CreatedAt.Year == year)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            model.CostThisMonth = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0 &&
                            t.CreatedDate.Month == month &&
                            t.CreatedDate.Year == year)
                .Sum(t => -(t.QuantityChange) * (t.CostPrice ?? 0));

            model.ProfitThisMonth = model.RevenueThisMonth - model.CostThisMonth;

            // ====== Chi tiết theo sản phẩm ======
            var revenueByProduct = _context.OrderItems
                .Include(x => x.Product)
                .GroupBy(x => new { x.ProductID, x.Product.ProductName })
                .Select(g => new ProductRevenueVM
                {
                    ProductID = g.Key.ProductID,
                    ProductName = g.Key.ProductName,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity),
                    QuantitySold = g.Sum(x => x.Quantity)
                }).ToList();

            var costByProduct = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0)
                .GroupBy(t => t.ProductID)
                .Select(g => new
                {
                    ProductID = g.Key,
                    Cost = g.Sum(t => -(t.QuantityChange) * (t.CostPrice ?? 0))
                }).ToList();

            // Join để tính profit
            foreach (var p in revenueByProduct)
            {
                var cost = costByProduct.FirstOrDefault(c => c.ProductID == p.ProductID)?.Cost ?? 0;
                p.Cost = cost;
                p.Profit = p.Revenue - cost;
            }

            model.ProductRevenue = revenueByProduct;

            // ====== Chi tiết theo tháng ======
            var revenueByMonth = _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenueVM
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                }).ToList();

            var costByMonth = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0)
                .GroupBy(t => new { t.CreatedDate.Year, t.CreatedDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Cost = g.Sum(t => -(t.QuantityChange) * (t.CostPrice ?? 0))
                }).ToList();

            foreach (var m in revenueByMonth)
            {
                var c = costByMonth.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Cost ?? 0;
                m.Cost = c;
                m.Profit = m.Revenue - c;
            }

            model.MonthlyRevenue = revenueByMonth
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ToList();

            // ====== Dashboard kho ======
            model.TotalProducts = _context.Products.Count();
            model.TotalStock = _context.Products.Sum(x => x.Quantity ?? 0);
            model.TotalImportThisMonth = _context.InventoryTransactions
                .Where(t => t.QuantityChange > 0 &&
                            t.CreatedDate.Month == month &&
                            t.CreatedDate.Year == year)
                .Sum(t => t.QuantityChange);
            model.TotalExportThisMonth = _context.InventoryTransactions
                .Where(t => t.QuantityChange < 0 &&
                            t.CreatedDate.Month == month &&
                            t.CreatedDate.Year == year)
                .Sum(t => -t.QuantityChange);
           

            return View(model);
        }
    }
}
