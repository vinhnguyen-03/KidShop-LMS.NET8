using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class InventoryController : Controller
    {
        private readonly InventoryService _inventoryService;
        private readonly DataContext _context;

        public InventoryController(InventoryService inventoryService, DataContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        // ====== Nhập kho ======
        [HttpGet]
        public IActionResult ImportStock()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ImportStock(int productId, int quantity, decimal costPrice, string? reason)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Số lượng phải lớn hơn 0");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            if (costPrice <= 0)
            {
                ModelState.AddModelError("", "Giá vốn phải > 0");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            var ok = _inventoryService.ImportStock(productId, quantity, costPrice, reason ?? "Nhập hàng");

            if (!ok)
            {
                ModelState.AddModelError("", "Không thể nhập kho");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            return RedirectToAction("History");
        }

        // ====== Xuất kho ======
        [HttpGet]
        public IActionResult ExportStock()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ExportStock(int productId, int quantity, string? reason)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Số lượng phải lớn hơn 0");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            var ok = _inventoryService.ExportStock(productId, quantity, reason ?? "Xuất hàng");

            if (!ok)
            {
                ModelState.AddModelError("", "Không đủ số lượng để xuất");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            return RedirectToAction("History");
        }

        // ====== Lịch sử kho ======
        public IActionResult History()
        {
            var data = _context.InventoryTransactions
                .Include(x => x.Product)
                .OrderByDescending(x => x.TransactionID)
                .ToList();

            return View(data);
        }
    }
}
