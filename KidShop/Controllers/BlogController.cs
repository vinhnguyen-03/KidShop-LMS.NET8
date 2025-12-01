using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel;
using KidShop.ViewModel.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly DataContext _context;
        private readonly GeminiService _geminiService;
        public BlogController(DataContext context, GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }
        public IActionResult Index(string keyword = "", int page = 1, int pageSize = 6)
        {
            var blogs = _context.Blogs
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .AsQueryable();

            // --- Tìm kiếm ---
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                blogs = blogs.Where(b => b.Title != null && b.Title.ToLower().Contains(lowerKeyword));
            }

            //  Phân trang
            var totalRecords = blogs.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = blogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách bài viết gan nhat
            var otherBlogs = _context.Blogs
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .ToList();

            // 5. Truyền dữ liệu sang View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;
            ViewBag.OtherBlogs = otherBlogs;

            return View(data);
        }
        // detail
        public IActionResult BLogDetail(int id)
        {
                var blog = _context.Blogs
             .Include(b => b.User) //   lấy thêm thông tin người dùng
             .FirstOrDefault(b => b.BlogID == id && b.IsActive == true);

            if (blog == null)
            {
                return NotFound();
            }
            // tang lượt xem mỗi lần vao
            blog.ViewCount++;
            _context.SaveChanges();

            // Lấy danh sách bài viết khác (trừ bài viết hiện tại)
            var otherBlogs = _context.Blogs
                .Where(b => b.BlogID != id)
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .ToList();
            ViewBag.OtherBlogs = otherBlogs;

            // Lấy comment + tên người dùng
            var comments = (from c in _context.Comments
                            join u in _context.Users on c.UserID equals u.UserID into userJoin
                            from u in userJoin.DefaultIfEmpty()
                            where c.TargetID == id && c.TargetType == "Blog" && c.IsActive
                            orderby c.CreatedDate descending
                            select new CommentWithUserVM
                            {
                                CommentID = c.CommentID,
                                Contents = c.Contents,
                                CreatedDate = c.CreatedDate,
                                FullName = u != null ? u.FullName : "Ẩn danh"
                            }).ToList();
            // Kiểm tra bài viết đã được lưu bởi user hiện tại hay chưa
            bool isSaved = false;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("UserID")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                isSaved = _context.SavedBlogs.Any(s => s.UserID == userId && s.BlogID == id);
            }

            var viewModel = new BlogDetailVM
            {
                Blog = blog,
                Comments = comments ?? new List<CommentWithUserVM>(),
                  BlogIsSaved = isSaved
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddComment(int blogId, string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                TempData["CommentError"] = "Nội dung bình luận không được để trống.";
                return RedirectToAction("BlogDetail", new { id = blogId });
            }

            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                // ✅ Gửi thông báo lỗi
                TempData["CommentError"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToAction("BlogDetail", new { id = blogId });
            }

            int userId = int.Parse(userIdClaim.Value);

            var comment = new tbl_Comment
            {
                TargetID = blogId,
                TargetType = "Blog",
                UserID = userId,
                Contents = contents,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            TempData["CommentSuccess"] = "Bình luận của bạn đã được gửi!";
            return RedirectToAction("BlogDetail", new { id = blogId });
        }

        // Tóm tắt bài viết
    
        [HttpPost]
        public async Task<IActionResult> SummarizeAI(int blogId)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(blogId);
                if (blog == null || string.IsNullOrEmpty(blog.Contents))
                    return Json(new { success = false, message = "Bài viết không tồn tại hoặc trống." });
 
                string summary = await _geminiService.SummarizeAsync(blog.Contents);

                return Json(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi tóm tắt: {ex.Message}" });
            }
        }

        // Chatbot
   
        [HttpPost]
        public async Task<IActionResult> AskAI(int blogId, string question)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(question))
                    return Json(new { success = false, message = "Câu hỏi không được để trống." });

                var blog = await _context.Blogs.FindAsync(blogId);
                if (blog == null || string.IsNullOrEmpty(blog.Contents))
                    return Json(new { success = false, message = "Bài viết không tồn tại hoặc trống." });

                string answer = await _geminiService.AskQuestionAsync(blog.Contents, question.Trim());
                return Json(new { success = true, answer });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi hỏi AI: {ex.Message}" });
            }
        }
    }
}
