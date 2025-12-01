using KidShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Controllers
{
    public class PaymentCourseController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public PaymentCourseController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Trang hướng dẫn chuyển khoản + QR tự động
        public IActionResult BankTransfer(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
                return BadRequest("OrderCode không hợp lệ.");

            var enrollment = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .FirstOrDefault(e => e.OrderCode == orderCode);

            if (enrollment == null)
                return NotFound("Không tìm thấy đơn hàng.");

            // Lấy cấu hình ngân hàng trong appsettings.json
            var bank = _config.GetSection("BankConfig");
            string bankCode = bank["BankCode"];
            string accountNo = bank["AccountNo"];
            string template = bank["Template"];
            string accountName = bank["AccountName"];

            // Tạo QR từ VietQR
            long amount = (long)enrollment.Course.Price;
            string addInfo = enrollment.OrderCode;

            string qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNo}-{template}.png?amount={amount}&addInfo={addInfo}";

            // Gửi ảnh QR sang View
            ViewBag.QRImageUrl = qrUrl;

            // Gửi cả thông tin ngân hàng sang View
            ViewBag.BankName = bankCode;
            ViewBag.AccountNo = accountNo;
            ViewBag.AccountName = accountName;

            return View(enrollment);
        }

    }
}
