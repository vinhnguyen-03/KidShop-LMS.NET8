using KidShop.Models;
using KidShop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;

namespace KidShop.Controllers
{
    [Authorize] // yêu cầu đăng nhập
    public class CartController : Controller
    {
        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

        //   Hàm lấy hoặc tạo giỏ hàng của người dùng
        private async Task<tbl_Cart> GetOrCreateUserCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                cart = new tbl_Cart
                {
                    UserID = userId,
                     Items = new List<tbl_CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        // Lấy UserID từ Claims (Identity)
        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserID");
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        //  Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                // Nếu là request AJAX → trả JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Bạn cần đăng nhập." });
                return RedirectToAction("Login", "User");
            }

            if (quantity <= 0) quantity = 1;

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                return NotFound("Sản phẩm không tồn tại.");
            }

            // CHECK TỒN KHO
            if (product.Quantity <= 0)
            {
                string url = Functions.TitleSlugGeneration("sanpham", product.ProductName ?? "no-title", product.ProductID);
         
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng." });

                TempData["ErrorMessage"] = "Sản phẩm đã hết hàng.";
                return Redirect("/" + url);
            }

            var cart = await GetOrCreateUserCartAsync(userId.Value);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductID == productId);

            int currentInCart = existingItem?.Quantity ?? 0;
            int totalAfterAdd = currentInCart + quantity;

            // ❗ CHECK VƯỢT TỒN KHO
            if (totalAfterAdd > product.Quantity)
            {
                string url = Functions.TitleSlugGeneration("sanpham", product.ProductName ?? "no-title", product.ProductID);
                string msg = $"Số lượng yêu cầu vượt quá tồn kho. Hiện chỉ còn {product.Quantity} sản phẩm.";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = msg });

                TempData["ErrorMessage"] = msg;
                return Redirect("/" + url);
            }

            // THÊM HOẶC CỘNG DỒN
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                _context.CartItems.Add(new tbl_CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                });

            await _context.SaveChangesAsync();

            // Tổng số lượng trong giỏ
            int totalQuantity = cart.Items.Sum(i => i.Quantity);

            // Nếu là AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    message = "🛒 Đã thêm vào giỏ hàng!",
                    cartCount = totalQuantity
                });
            }

            TempData["SuccessMessage"] = "🛒 Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Index");
        }


        //  Cập nhật số lượng sản phẩm trong giỏ
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
                quantity = 1;

            var userId = GetUserId();
            if (userId == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập." });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
                return Json(new { success = false, message = "Không tìm thấy giỏ hàng." });

            var item = cart.Items.FirstOrDefault(x => x.ProductID == productId);
            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ." });

            var product = item.Product;

            // CHECK VƯỢT TỒN KHO
            if (quantity > product.Quantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"Chỉ còn {product.Quantity} cái!",
                    max = product.Quantity
                });
            }

            item.Quantity = quantity;
            await _context.SaveChangesAsync();

            //  Ép kiểu về decimal để tránh lỗi ToString
            decimal price = (item.Product.PriceSale ?? item.Product.Price) ?? 0;
            decimal itemTotal = price * item.Quantity;
            decimal subtotal = cart.Items.Sum(i => ((i.Product.PriceSale ?? i.Product.Price) ?? 0) * i.Quantity);

            return Json(new
            {
                success = true,
                itemTotal = $"{itemTotal:N0} đ",
                subtotal = $"{subtotal:N0} đ"
            });
        }

        //   Hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            return View(cart ?? new tbl_Cart { Items = new List<tbl_CartItem>() });
        }

        // Xóa 1 sản phẩm khỏi giỏ
        public async Task<IActionResult> RemoveCart(int id)
        {
            var userId = GetUserId();
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemID == id && i.Cart != null && i.Cart.UserID == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
