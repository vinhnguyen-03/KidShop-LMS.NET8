using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly InventoryService _inventoryService;

        public OrderController(DataContext context, InventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            int userId = GetCurrentUserId();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutVM
            {
                Items = cart.Items.Select(i => new OrderItemVM
                {
                    ProductID = i.ProductID,
                    ProductName = i.Product.ProductName,
                    // Ưu tiên giá sale nếu có
                    Price = i.Product.PriceSale ?? i.Product.Price ?? 0m,
                    Quantity = i.Quantity
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            int userId = GetCurrentUserId();

            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra giỏ hàng trong DB (tránh gian lận client)
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            // Khởi tạo đơn hàng
            var order = new tbl_Order
            {
                UserID = userId,
                ReceiverName = model.ReceiverName,
                ShippingAddress = model.Address,
                PhoneNumber = model.Phone,
                PaymentMethod = model.PaymentMethod,
                Status = model.PaymentMethod == "Bank" ? "Đã thanh toán" : "Chờ xử lý",
                Email = model.Email,
                Note = model.Note,
                CreatedAt = DateTime.Now,

                TotalAmount = 0m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // cần để có OrderID

            decimal total = 0m;

            // Duyệt qua sản phẩm trong giỏ
            foreach (var cartItem in cart.Items)
            {
                var price = cartItem.Product.PriceSale ?? cartItem.Product.Price ?? 0m;

                var orderItem = new tbl_OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = cartItem.ProductID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = price
                };
                _context.OrderItems.Add(orderItem);

                total += price * cartItem.Quantity;

                // ✅ Cập nhật tồn kho qua InventoryService
                _inventoryService.ExportStock(
                    cartItem.ProductID,
                    cartItem.Quantity,
                    $"Xuất đơn hàng #{order.OrderID}"
                );
            }

            // Cập nhật tổng tiền sau khi duyệt xong
            order.TotalAmount = total;

            // Xóa giỏ hàng
            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success));
        }

        private int GetCurrentUserId()
        {
            // Lấy UserID từ Claims (nếu đăng nhập bằng Identity)
            return int.Parse(User.FindFirst("UserID").Value);
        }

        public IActionResult Success()
        {
            return View();
        }

        //  Xem danh sách đơn hàng đã đặt
        [HttpGet]
        public async Task<IActionResult> HistoryOrder()
        {
            int userId = GetCurrentUserId();

            var orders = await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        //  Xem chi tiết 1 đơn hàng
        [HttpGet]
        public async Task<IActionResult> DetailOrder(int id)
        {
            int userId = GetCurrentUserId();

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderID == id && o.UserID == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
