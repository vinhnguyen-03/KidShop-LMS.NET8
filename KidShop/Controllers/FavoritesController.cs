using KidShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidShop.Controllers
{
    [Authorize] // yêu cầu đăng nhập
    public class FavoritesController : Controller
    {
        private readonly DataContext _context;
        public FavoritesController(DataContext context)
        {
            _context = context;
        }
        // Lấy UserID từ Claims (Identity)
        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserID");
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        //  Danh sách yêu thích
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var favorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserID == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            bool exists = await _context.Favorites
                .AnyAsync(f => f.UserID == userId && f.ProductID == productId);

            if (!exists)
            {
                _context.Favorites.Add(new tbl_Favorites
                {
                    UserID = userId.Value,
                    ProductID = productId
                });
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "💖 Đã thêm sản phẩm vào danh sách yêu thích!";
            }
            else
            {
                TempData["ErrorMessage"] = "⚠️ Sản phẩm này đã có trong danh sách yêu thích!";
            }
             
            return Redirect(Request.Headers["Referer"].ToString() ?? Url.Action("Index", "Favorites"));
        }
        //  Xóa khỏi yêu thích
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var item = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);

            if (item != null)
            {
                _context.Favorites.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Message"] = "❌ Đã xóa sản phẩm khỏi danh sách yêu thích!";
            }

            return RedirectToAction("Index");
        }
    }
}
