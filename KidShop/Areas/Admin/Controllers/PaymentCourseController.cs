using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class PaymentCourseController : Controller
    {
        private readonly DataContext _context;
        public PaymentCourseController(DataContext context)
        {
            _context = context;
        }
        // Danh sách đơn đăng ký
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Course.CourseName.Contains(search)
                                      || e.User.FullName.Contains(search)
                                      || e.OrderCode.Contains(search));
            }

            query = query.OrderByDescending(e => e.RegisterDate);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedData = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(pagedData);
        }

        // Xác nhận đã nhận tiền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmPayment(int id)
        {
            var enroll = _context.Enrollments.FirstOrDefault(e => e.EnrollmentID == id);
            if (enroll == null) return NotFound();

            enroll.IsPaid = true;
            _context.SaveChanges();

            TempData["Message"] = "Xác nhận thanh toán thành công!";
            return RedirectToAction("Index");
        }
    }
}
