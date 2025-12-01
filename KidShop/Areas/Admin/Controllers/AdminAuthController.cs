using KidShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AdminAuthController : Controller
    {
        private readonly DataContext _context;
        public AdminAuthController(DataContext context)
        {
            _context = context;
        }
        // GET: Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminID") != null)
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == "Admin");

            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại!";
                return View();
            }

            // Nếu tài khoản đang bị khóa
            if (user.IsLocked == true)
            {
                var minutesLeft = (user.LockoutEndTime.Value - DateTime.Now).TotalMinutes;
                ViewBag.Error = $"Tài khoản bị khóa còn {Math.Ceiling(minutesLeft)} phút";
                return View();
            }

            // Kiểm tra mật khẩu bằng BCrypt
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                user.FailedLoginAttempts += 1;

                // Sai 5 lần → khóa 10 phút
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEndTime = DateTime.Now.AddMinutes(10);
                }

                await _context.SaveChangesAsync();
                ViewBag.Error = "Mật khẩu không đúng!";
                return View();
            }

            // Đăng nhập thành công
            user.FailedLoginAttempts = 0;
            user.LockoutEndTime = null;
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Lưu session
            HttpContext.Session.SetString("AdminID", user.UserID.ToString());
            HttpContext.Session.SetString("AdminName", user.FullName ?? user.Username);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
