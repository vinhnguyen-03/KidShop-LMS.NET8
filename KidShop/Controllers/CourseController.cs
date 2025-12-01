using KidShop.Models;
using KidShop.ViewModel.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidShop.Controllers
{
    public class CourseController : Controller
    {
        private DataContext _context;
        public CourseController(DataContext context)
        {
            _context = context;
        }
        // danh sach khoa h
        public IActionResult Index(int page = 1, int pageSize = 9, string keyword = "")
        {
            var course = _context.Courses
                .Include(c => c.Lecturers)
               .Where(c => c.IsActive)
               .AsQueryable();
            // --- Tìm kiếm ---
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                course = course.Where(c => c.CourseName != null && c.CourseName.ToLower().Contains(lowerKeyword));
            }
            //  tong ban ghi, Phân trang
            var totalRecords = course.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = course
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 5. Truyền dữ liệu sang View
            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;


            return View(data);
        }
        // hien thi chi tiêt khó học
        public IActionResult CourseDetail(int id)
        {
             
            var course = _context.Courses
                .Include(c => c.Lecturers) // nạp giảng viên (nếu có FK)
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            // Lấy danh sách video thuộc khóa học
            var videos = _context.CourseVideos
                .Where(v => v.CourseID == id && v.IsActive)
                .OrderBy(v => v.OrderIndex)
                .ToList();

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.LecturerID == course.LecturerID);


            var viewModel = new CourseDetailVM
            {
                Course = course,
                Lecturer = lecturer,
                Videos = videos,
                TotalVideos = videos.Count,
                FirstVideo = videos.FirstOrDefault()
            };

            return View(viewModel);
        }
        // Controller cho Xem thử
        public IActionResult CoursePreview(int id)
        {
            var video = _context.CourseVideos.Find(id);
            if (video == null) return NotFound();

            if (!video.IsPreview)
                return RedirectToAction("CourseDetail", new { id = video.CourseID });// chặn nếu không phải video học thử

            return View(video); // Hiển thị video preview
        }
        //dang ky khoa hoc
        [HttpPost]
        [Authorize] // Bắt buộc đăng nhập
        public async Task<IActionResult> Register(int courseId)
        {
            // Lấy UserID từ Claims (Identity)
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                // Nếu không có claim UserID → quay lại login
                return RedirectToAction("Login", "Account", new
                {
                    returnUrl = Url.Action("CourseDetail", new { id = courseId })
                });
            }

            int userId = int.Parse(userIdClaim.Value);

            // Kiểm tra khóa học tồn tại
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound();

            // Kiểm tra đã đăng ký chưa
            var existingEnroll = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseID == courseId && e.UserID == userId);

            if (existingEnroll != null)
            {
                TempData["Message"] = "Bạn đã đăng ký khóa học này!";
                return RedirectToAction("CourseIndex", new { id = courseId });
            }

            // Nếu khóa học miễn phí
            if (course.IsFree || (course.Price ?? 0) <= 0)
            {
                var enroll = new tbl_Enrollment
                {
                    CourseID = courseId,
                    UserID = userId,
                    IsPaid = true,
                    RegisterDate = DateTime.Now
                };

                _context.Enrollments.Add(enroll);
                await _context.SaveChangesAsync();

                TempData["Message"] = "🎉 Đăng ký khóa học miễn phí thành công!";
                return RedirectToAction("CourseIndex", new { id = courseId });
            }
            else
            {
                // Nếu khóa học trả phí
                string orderCode = $"KH-C{courseId:D3}-U{userId:D3}-{DateTime.Now:yyyyMMddHHmmss}";
                var enroll = new tbl_Enrollment
                {
                    CourseID = courseId,
                    UserID = userId,
                    IsPaid = false,
                    OrderCode = orderCode,
                    RegisterDate = DateTime.Now
                };

                _context.Enrollments.Add(enroll);
                await _context.SaveChangesAsync();

                return RedirectToAction("BankTransfer", "PaymentCourse", new { orderCode });
            }
        }

        //chi tiết các bài học của khoá học
        [Authorize]
        public IActionResult CourseIndex(int id, int? videoId)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("CourseIndex", new { id }) });

            int userId = int.Parse(userIdClaim.Value);

            // Kiểm tra đã đăng ký và thanh toán (nếu có)
            var enrollment = _context.Enrollments
                        .Include(e => e.Course)
                        .FirstOrDefault(e => e.CourseID == id && e.UserID == userId && (e.IsPaid || e.Course.IsFree));

            if (enrollment == null)
            {
                TempData["Message"] = "❌ Bạn cần đăng ký khóa học trước khi xem bài học!";
                return RedirectToAction("CourseDetail", new { id });
            }

            // Lấy thông tin khóa học kèm giảng viên
            var course = _context.Courses
                .Include(c => c.Lecturers)
                .FirstOrDefault(c => c.CourseID == id && c.IsActive);

            if (course == null) return NotFound();

            // Lấy danh sách video theo thứ tự OrderIndex
            var videos = _context.CourseVideos
                .Where(v => v.CourseID == id && v.IsActive)
                .OrderBy(v => v.OrderIndex)
                .ToList();

            // Xác định video hiện tại: nếu videoId có giá trị thì lấy video đó, ngược lại lấy video đầu tiên
            var currentVideo = videoId.HasValue
                ? videos.FirstOrDefault(v => v.CourseVideoID == videoId.Value) ?? videos.FirstOrDefault()
                : videos.FirstOrDefault();
            //  Load danh sách quiz cho video hiện tại
            var quizzes = currentVideo != null
                ? _context.Quizzes.Where(q => q.CourseVideoID == currentVideo.CourseVideoID).ToList()
                : new List<tbl_Quiz>();

            var viewModel = new CourseDetailVM
            {
                Course = course,
                Lecturer = course.Lecturers,
                Videos = videos,
                FirstVideo = currentVideo,
                TotalVideos = videos.Count,
                Quizzes = quizzes
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GetQuizByVideo(int videoId)
        {
            var quizzes = _context.Quizzes
                .Where(q => q.CourseVideoID == videoId)
                .ToList();

            return PartialView("_QuizPartial", quizzes);
        }
    }
}
