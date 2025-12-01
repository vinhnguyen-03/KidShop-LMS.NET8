using KidShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using BCrypt.Net;
using KidShop.ViewModel.User;

namespace KidShop.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Username == model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                }
                else if (user.Status == false)
                {
                    ModelState.AddModelError("", " Tài khoản đã bị khóa do ad");
                }
                else if (user.LockoutEndTime != null && user.LockoutEndTime > DateTime.Now)
                {
                    ModelState.AddModelError("", $"Tài khoản đang bị tạm khóa đến {user.LockoutEndTime?.ToString("HH:mm:ss dd/MM/yyyy")}");
                }
                else
                {
                    // Kiểm tra mật khẩu bằng BCrypt
                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Passwords, user.PasswordHash);

                    if (!isPasswordCorrect)
                    {
                        user.FailedLoginAttempts++;

                        if (user.FailedLoginAttempts >= 3)
                        {
                            user.LockoutEndTime = DateTime.Now.AddMinutes(5); // Khóa 5 phút
                            ModelState.AddModelError("", "Tài khoản đã bị khóa tạm thời do nhập sai nhiều lần. Vui lòng thử lại sau.");
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Sai mật khẩu. Bạn còn {3 - user.FailedLoginAttempts} lần thử.");
                        }

                        _context.SaveChanges();
                    }
                    else
                    {
                        // Đăng nhập thành công
                        user.FailedLoginAttempts = 0;
                        user.LockoutEndTime = null;
                        user.LastLogin = DateTime.Now;
                        _context.SaveChanges();

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),  //  Username để định danh
                            new Claim("FullName", user.FullName ?? ""),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim("UserID", user.UserID.ToString()),
                            new Claim(ClaimTypes.Role, user.Role ?? "User")
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return Url.IsLocalUrl(ReturnUrl)
                            ? Redirect(ReturnUrl)
                            : RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                var existUser = _context.Users.FirstOrDefault(u => u.Username == model.UserName);
                if (existUser != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                // Kiểm tra email đã được sử dụng
                var existEmail = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }
                // Hash mật khẩu bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Passwords);

                var user = new tbl_User
                {
                    Username = model.UserName,
                    PasswordHash = hashedPassword, // Lưu mật khẩu đã hash
                    FullName = model.FullName,
                    Email = model.Email,
                    CreatedAt = DateTime.Now,
                    Status = true,
                    Role = "User",
                    FailedLoginAttempts = 0,
                    LockoutEndTime = null,
                    ResetOtp = null,
                    ResetOtpExpiry = null
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordVM model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống!");
                return View(model);
            }

            // ✅ Tạo mã OTP 6 số
            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.ResetOtpExpiry = DateTime.Now.AddMinutes(5); // hết hạn sau 5 phút
            _context.SaveChanges();

            // ✅ Gửi email OTP
            SendOtpEmail(user.Email, otp);

            // Lưu email vào TempData để chuyển qua trang xác nhận OTP
            TempData["Email"] = user.Email;
            return RedirectToAction("VerifyOtp");
        }

        private void SendOtpEmail(string toEmail, string otp)
        {
            string emailUser = _configuration["EmailConfig:EmailUsername"];
            string emailPass = _configuration["EmailConfig:EmailPassword"];

            var message = new MailMessage();
            message.From = new MailAddress(emailUser, "KidShop Support");
            message.To.Add(toEmail);
            message.Subject = "Mã xác nhận khôi phục mật khẩu";
            message.Body = $"Mã xác nhận của bạn là: {otp}\nMã sẽ hết hạn sau 5 phút.";

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(emailUser, emailPass);
                client.Send(message);
            }
        }
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpVM model)
        {
            //   Tìm người dùng theo email
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy email trong hệ thống.");
                return View(model);
            }

            //  Kiểm tra xem có mã OTP chưa
            if (string.IsNullOrEmpty(user.ResetOtp) || user.ResetOtpExpiry == null)
            {
                ModelState.AddModelError("", "Chưa có mã OTP hoặc mã không hợp lệ.");
                return View(model);
            }

            //  Kiểm tra mã OTP có đúng và còn hạn không
            bool otpSai = user.ResetOtp != model.OtpCode;
            bool otpHetHan = user.ResetOtpExpiry < DateTime.Now;

            if (otpSai || otpHetHan)
            {
                ModelState.AddModelError("", otpHetHan ? "Mã OTP đã hết hạn!" : "Mã OTP không đúng!");
                return View(model);
            }

            //   OTP hợp lệ → cho phép đặt lại mật khẩu
            TempData["Email"] = user.Email;

            // (tuỳ chọn) Xoá OTP sau khi xác nhận
            user.ResetOtp = null;
            user.ResetOtpExpiry = null;
            _context.SaveChanges();

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = TempData["Email"] as string;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }
            var model = new ResetPasswordVM { Email = email };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordVM model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp!");
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null) return NotFound();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetOtp = null;
            user.ResetOtpExpiry = null;
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = User.Identity?.Name; // hoặc lấy từ session
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login", "User");

            var model = new ProfileVM
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email  
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(ProfileVM model)
        {
            if (!ModelState.IsValid)
                return View("Profile", model);

            var user = _context.Users.Find(model.UserID);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

            return RedirectToAction("Profile", "User");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            // Kiểm tra mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng !");
                return View(model);
            }

            // Kiểm tra trùng lặp
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", " Mật khẩu xác nhận không khớp!.");
                return View(model);
            }

            // Hash mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
