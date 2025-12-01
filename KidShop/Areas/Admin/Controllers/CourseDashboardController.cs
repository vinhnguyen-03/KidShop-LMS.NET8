using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Course;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CourseDashboardController : Controller
    {
        private DataContext _context;
        public CourseDashboardController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10, int year = 0)
        {
            if (year == 0) year = DateTime.Now.Year;

            // Tổng số khóa học trả phí
            ViewBag.TotalCourses = _context.Courses.Where(c => !c.IsFree).Count();

            // Query: chỉ lấy khóa học trả phí + include enrollments
            var query = _context.Courses
                .Where(c => !c.IsFree)                     // ❌ loại khóa miễn phí
                .Include(c => c.Enrollments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search));
            }

            // Lấy ra client xử lý MonthlyRevenue
            var data = query
                .AsEnumerable()
                .Select(c => new CourseStatisticVM
                {
                    CourseID = c.CourseID,
                    CourseName = c.CourseName,

                    // Tổng học viên đăng ký (kể cả chưa thanh toán)
                    StudentCount = c.Enrollments.Count(),

                    // ❗ Chỉ tính doanh thu khi IsPaid == true
                    TotalRevenue = c.Enrollments
                        .Where(e => e.IsPaid)
                        .Sum(e => c.Price ?? 0),

                    MonthlyRevenue = Enumerable.Range(1, 12)
                        .ToDictionary(
                            m => m,
                            m => c.Enrollments
                                .Where(e =>
                                    e.IsPaid &&                          
                                    e.RegisterDate.Year == year &&
                                    e.RegisterDate.Month == m
                                )
                                .Sum(e => c.Price ?? 0)
                        )
                })
                .OrderByDescending(c => c.StudentCount)
                .ToList();

            // Tổng doanh thu tất cả khóa học trả phí
            ViewBag.TotalRevenue = data.Sum(d => d.TotalRevenue);

            // Phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.Year = year;

            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Chart.js data
            var chartData = new
            {
                labels = Enumerable.Range(1, 12)
                    .Select(m => System.Globalization.CultureInfo.CurrentCulture
                        .DateTimeFormat.GetAbbreviatedMonthName(m))
                    .ToArray(),

                datasets = data.Select(c => new
                {
                    label = c.CourseName,
                    data = Enumerable.Range(1, 12).Select(m => c.MonthlyRevenue[m]).ToArray(),
                    backgroundColor = $"#{new Random().Next(0x1000000):X6}"
                })
            };

            ViewBag.ChartData = Newtonsoft.Json.JsonConvert.SerializeObject(chartData);

            return View(pagedData);
        }
    }
}
